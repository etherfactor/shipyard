using EtherGizmos.Common.Abstractions;
using Serilog.Formatting;
using Serilog.Formatting.Compact;

namespace EtherGizmos.Common.Services;

internal class TeeStreamAmbient : ITeeStreamScopeFactory
{
    private static readonly AsyncLocal<Ctx?> _ambient = new();
    internal static Ctx? Current => _ambient.Value;

    public IDisposable Begin(Stream stream, ITextFormatter? formatter = null)
    {
        var writer = new StreamWriter(stream, leaveOpen: true) { AutoFlush = true };
        var ctx = new Ctx(TextWriter.Synchronized(writer), formatter ?? new RenderedCompactJsonFormatter());
        _ambient.Value = ctx;
        return new Scope(() =>
        {
            try { writer.Dispose(); } finally { _ambient.Value = null; }
        });
    }

    internal sealed class Ctx
    {
        public Ctx(TextWriter writer, ITextFormatter formatter) { Writer = writer; Formatter = formatter; }
        public TextWriter Writer { get; }
        public ITextFormatter Formatter { get; }
    }

    private sealed class Scope : IDisposable
    {
        private readonly Action _end;

        private bool _disposed;

        public Scope(Action end) => _end = end;

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
