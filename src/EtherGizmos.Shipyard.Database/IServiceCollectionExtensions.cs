using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Configuration;
using EtherGizmos.Extensions.DependencyInjection;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Migrations.Core;
using EtherGizmos.Shipyard.Services;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Reflection;

namespace EtherGizmos.Shipyard;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection @this)
    {
        @this.AddOptions<DatabaseReferenceOptions>()
            .ValidateOnStart()
            .ValidateDataAnnotations();

        @this.AddSingleton<IMigrationManager, MigrationManager>()
            .AddSingleton<IOAuth2MigrationManager, MigrationManager>()
            .AddDbContext<ApplicationContext>((services, opt) =>
            {
                opt.UseLazyLoadingProxies();
                opt.EnableSensitiveDataLogging();

                var dbOptions = services.GetRequiredService<IOptions<DatabaseReferenceOptions>>()
                    .Value;

                var connectionId = dbOptions.ConnectionId;

                var resolver = services.GetRequiredService<IConnectionResolver>();
                var connection = resolver.GetDatabaseConnection(connectionId);

                connection.Match(
                    _ => throw new InvalidOperationException($"The connection {connectionId} is not a valid database connection."),
                    postgreSql =>
                    {
                        return opt.UseNpgsql(
                            postgreSql.ConnectionString,
                            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                    }
                );
            });

        @this
            .AddChildContainer((child, parent) =>
            {
                var dbOptions = parent.GetRequiredService<IOptions<DatabaseReferenceOptions>>()
                    .Value;

                var connectionId = dbOptions.ConnectionId;

                var resolver = parent.GetRequiredService<IConnectionResolver>();
                var connection = resolver.GetDatabaseConnection(connectionId);

                child.AddFluentMigratorCore()
                    .ConfigureRunner(opt =>
                    {
                        opt.ScanIn(typeof(ApplicationContext).Assembly).For.Migrations()
                            .WithVersionTable(new PostgresVersionTableMetadata());

                        connection.Match(
                            _ => throw new InvalidOperationException($"The connection {connectionId} is not a valid database connection."),
                            postgreSql => opt.AddPostgres()
                                .WithGlobalConnectionString(postgreSql.ConnectionString)
                        );
                    });
            })
            .ImportLogging()
            .ForwardScoped<IMigrationRunner>();

        return @this;
    }

    public static IServiceCollection AddUnitOfWork(
        this IServiceCollection @this,
        Action<UnitOfWorkOptions> configureOptions)
    {
        @this.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>()
            .AddScoped(typeof(IRepository<>), typeof(EfCoreRepository<>));

        @this.AddOptions<UnitOfWorkOptions>()
            .Configure(configureOptions);

        var options = new UnitOfWorkOptions();
        configureOptions(options);

        foreach (var pair in options.EntityContexts)
        {
            var entityType = pair.Key;
            var contextType = pair.Value;
            var serviceType = typeof(DbSet<>).MakeGenericType(entityType);

            @this.AddKeyedScoped(typeof(DbContext), entityType, contextType)
                .AddScoped(serviceType, provider =>
                {
                    var context = (DbContext)provider.GetRequiredService(contextType);
                    return context
                        .GetType()
                        .GetMethod(nameof(DbContext.Set), BindingFlags.Instance | BindingFlags.Public, [])!
                        .MakeGenericMethod(entityType)
                        .Invoke(context, [])!;
                });
        }

        return @this;
    }
}

public class UnitOfWorkOptions
{
    internal HashSet<Type> ContextTypes { get; } = [];
    internal ConcurrentDictionary<Type, Type> EntityContexts { get; } = [];

    public UnitOfWorkOptions BindDbContext<TContext>()
        where TContext : DbContext
    {
        if (!ContextTypes.Contains(typeof(TContext)))
        {
            lock (ContextTypes)
            {
                if (ContextTypes.Contains(typeof(TContext)))
                    return this;

                ContextTypes.Add(typeof(TContext));

                var entityTypes = typeof(TContext)
                    .GetProperties()
                    .Where(e => e.PropertyType.IsGenericType && e.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .Select(e => e.PropertyType.GetGenericArguments()[0])
                    .Distinct()
                    .ToList();

                var existing = entityTypes.Where(EntityContexts.ContainsKey);
                if (existing.Any())
                {
                    throw new InvalidOperationException($"The following types are already mapped in another context and " +
                        $"cannot be mapped to {typeof(TContext)}:\r\n{string.Join(", ", existing)}");
                }

                foreach (var type in entityTypes)
                {
                    EntityContexts.GetOrAdd(type, typeof(TContext));
                }
            }
        }

        return this;
    }
}
