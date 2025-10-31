using Serilog.Formatting;

namespace EtherGizmos.Common.Abstractions;

public interface ITeeStreamScopeFactory
{
    IDisposable Begin(Stream stream, ITextFormatter? formatter = null);
}
