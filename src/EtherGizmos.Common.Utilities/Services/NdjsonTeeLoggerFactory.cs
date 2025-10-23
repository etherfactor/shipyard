using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;

namespace EtherGizmos.Common.Services;

internal class NdjsonTeeLoggerFactory
{
    public Microsoft.Extensions.Logging.ILogger Create(
        Microsoft.Extensions.Logging.ILogger baseLogger,
        Stream ndjsonStream,
        string categoryName)
    {
        ArgumentNullException.ThrowIfNull(ndjsonStream);
        ArgumentNullException.ThrowIfNull(categoryName);

        var writer = new StreamWriter(ndjsonStream) { AutoFlush = true };
        var synchronized = TextWriter.Synchronized(writer);

        var formatter = new RenderedCompactJsonFormatter();

        var subLogger = new LoggerConfiguration()
            .WriteTo.TextWriter(textWriter: synchronized, formatter: formatter)
            .CreateLogger();

        var serilogFactory = new SerilogLoggerFactory(subLogger, dispose: true);

        var streamLogger = serilogFactory.CreateLogger(categoryName);

        var composite = new CompositeLogger(baseLogger, streamLogger);
    }
}

public class TeeLogger : IDisposable
{
    private readonly object[] _disposables;

    private bool _disposed;

    public Microsoft.Extensions.Logging.ILogger Logger { get; }

    internal TeeLogger(
        Microsoft.Extensions.Logging.ILogger logger,
        params object[] disposables)
    {
        Logger = logger;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                foreach (var disposable in _disposables)
                {
                    (disposable as IDisposable)?.Dispose();
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposed = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~TeeLogger()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
