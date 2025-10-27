using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.Common.Extensions;

internal static class OpenIddictExtensions
{
    public static OpenIddictEntityFrameworkCoreBuilder UseDbContext(
        this OpenIddictEntityFrameworkCoreBuilder @this,
        Type type)
    {
        typeof(OpenIddictEntityFrameworkCoreBuilder).GetMethod(nameof(OpenIddictEntityFrameworkCoreBuilder.UseDbContext))!
            .MakeGenericMethod(type)
            .Invoke(@this, []);

        return @this;
    }
}
