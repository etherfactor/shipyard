namespace EtherGizmos.Shipyard.Abstractions;

public interface ISourceLoggerFactory
{
    Serilog.ILogger GetLogger(string apiKey);
}
