using EtherGizmos.Common.Abstractions;
using Serilog.Formatting;
using Serilog.Formatting.Compact;

namespace EtherGizmos.Common.Services;

internal class TeeStreamAmbient : ITeeStreamScopeFactory
{
    private static readonly AsyncLocal<TeeContext?> _ambient = new();
    internal static TeeContext? Current => _ambient.Value;

    public IDisposable Begin(
        Stream stream,
        ITextFormatter? formatter = null)
    {
        //Wrap a writer around the stream, then store the writer and a formatter for writing back to the stream
        var writer = new StreamWriter(stream, leaveOpen: true) { AutoFlush = true };
        var ctx = new TeeContext(TextWriter.Synchronized(writer), formatter ?? new RenderedCompactJsonFormatter());

        //Storing this in an AsyncLocal allows us to capture logs for a single execution, without capturing other logs
        _ambient.Value = ctx;

        //When this is disposed, we clear out the writer, so we stop capturing to the stream
        return new TeeScope(() =>
        {
            try { writer.Dispose(); } finally { _ambient.Value = null; }
        });
    }

    internal sealed record TeeContext(TextWriter Writer, ITextFormatter Formatter);

    private sealed class TeeScope : IDisposable
    {
        private readonly Action _end;

        private bool _disposed;

        public TeeScope(Action end)
        {
            _end = end;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _end();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
