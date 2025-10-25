using Microsoft.Extensions.Logging;

namespace EtherGizmos.Common.Extensions;

public static class ILoggerExtensions
{
    public static IDisposable? BeginKeyedScope(
        this ILogger @this,
        string key,
        object? value)
        => @this.BeginScope(new Dictionary<string, object?>()
        {
            [key] = value,
        });
}
