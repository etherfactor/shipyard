#pragma warning disable IDE0130
namespace Microsoft.Extensions.Logging;

public static class ILoggerExtensions
{
    public static IDisposable? BeginScope(
        this ILogger @this,
        string key,
        object? value)
        => @this.BeginScope(new Dictionary<string, object?>()
        {
            [key] = value,
        });
}
