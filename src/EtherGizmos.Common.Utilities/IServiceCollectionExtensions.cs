using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EtherGizmos.Common;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddModelValidators(
        this IServiceCollection @this,
        Assembly? rootAssembly = null)
    {
        @this.TryAddScoped<IModelValidatorFactory, ModelValidatorFactory>();

        var modelValidators = GetAssemblies(rootAssembly ?? Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Somehow started without an assembly"))
            //GetTypes() throws an exception for Microsoft.Data.SqlClient until it updates to 5.2.0
            .SelectMany(a => { try { return a.GetTypes(); } catch { return []; } })
            .Where(t => !t.IsInterface && !t.IsAbstract && !t.IsGenericType)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IModelValidator<>))
                .Select(i => new { ValidatorType = t, ModelType = i.GetGenericArguments().Single() }));

        foreach (var modelValidator in modelValidators)
        {
            var serviceType = typeof(IModelValidator<>).MakeGenericType(modelValidator.ModelType);
            @this.TryAddScoped(serviceType, modelValidator.ValidatorType);
        }

        return @this;
    }

    public static IServiceCollection AddTeeStreamLogger(
        this IServiceCollection @this)
    {
        @this.AddSingleton<ILoggerProvider, TeeStreamLoggerProvider>();
        @this.AddSingleton<ITeeStreamScopeFactory, TeeStreamAmbient>();
        return @this;
    }

    private static IEnumerable<Assembly> GetAssemblies(
        Assembly root)
    {
        var assemblies = new Dictionary<string, Assembly>();

        var referencedAssemblies = root
            .GetReferencedAssemblies()
            .Where(e => e.Name?.StartsWith("System.") == false)
            .Select(Assembly.Load);

        foreach (var assembly in referencedAssemblies)
        {
            if (assemblies.TryAdd(assembly.ToString(), assembly))
            {
                var subAssemblies = GetAssemblies(assembly);
                foreach (var subAssembly in subAssemblies)
                {
                    assemblies.TryAdd(subAssembly.ToString(), subAssembly);
                }
            }
        }

        return assemblies.Select(e => e.Value);
    }
}
