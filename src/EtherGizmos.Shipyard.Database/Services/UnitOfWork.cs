using EtherGizmos.Common.Utilities.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace EtherGizmos.Shipyard.Database.Services;

internal class UnitOfWork : IUnitOfWork
{
    private readonly IOptions<UnitOfWorkOptions> _options;
    private readonly IServiceScope? _serviceScope;
    private readonly IServiceProvider _serviceProvider;

    private readonly ConcurrentDictionary<Type, DbContext> _contexts = [];

    private bool _disposed;

    public UnitOfWork(
        IOptions<UnitOfWorkOptions> options,
        IServiceScope serviceScope)
        : this(options, serviceScope.ServiceProvider)
    {
        _serviceScope = serviceScope;
    }

    public UnitOfWork(
        IOptions<UnitOfWorkOptions> options,
        IServiceProvider serviceProvider)
    {
        _options = options;
        _serviceProvider = serviceProvider;
    }

    public IRepository<TEntity> Repository<TEntity>()
        where TEntity : class, IEntity
    {
        LoadContext<TEntity>();

        var repository = _serviceProvider.GetRequiredService<IRepository<TEntity>>();
        return repository;
    }

    private DbContext LoadContext<TEntity>()
        where TEntity : class
    {
        var contextType = _options.Value.EntityContexts[typeof(TEntity)];
        var context = _contexts.GetOrAdd(contextType, type =>
        {
            var context = (DbContext)_serviceProvider.GetRequiredService(type);
            lock (context.Database)
            {
                if (context.Database.CurrentTransaction is null)
                    context.Database.BeginTransaction();
            }

            return context;
        });

        return context;
    }

    public int SaveChanges()
    {
        var total = 0;
        var exceptions = new List<Exception>();

        var parallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 8,
        };

        Parallel.ForEach(_contexts.Values, parallelOptions, (context) =>
        {
            try
            {
                total += context.SaveChanges();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        });

        if (exceptions.Any())
        {
            throw new AggregateException(
                "Encountered an exception while saving database changes.",
                exceptions);
        }

        Parallel.ForEach(_contexts.Values, parallelOptions, (context) =>
        {
            try
            {
                context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        });

        if (exceptions.Any())
        {
            throw new AggregateException(
                "Encountered an exception while committing database changes. Data may be in an unexpected state.",
                exceptions);
        }

        return total;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var total = 0;
        var exceptions = new List<Exception>();

        var parallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 8,
            CancellationToken = cancellationToken,
        };

        await Parallel.ForEachAsync(_contexts.Values, parallelOptions, async (context, cancellationToken) =>
        {
            try
            {
                total += await context.SaveChangesAsync(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        });

        if (exceptions.Any())
        {
            throw new AggregateException(
                "Encountered an exception while saving database changes.",
                exceptions);
        }

        await Parallel.ForEachAsync(_contexts.Values, parallelOptions, async (context, cancellationToken) =>
        {
            try
            {
                await context.Database.CommitTransactionAsync(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        });

        if (exceptions.Any())
        {
            throw new AggregateException(
                "Encountered an exception while committing database changes. Data may be in an unexpected state.",
                exceptions);
        }

        return total;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _serviceScope?.Dispose();
            }

            _disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
