namespace EtherGizmos.Shipyard.Api.Abstractions;

public interface ISourceLoggerFactory
{
    Serilog.ILogger GetLogger(string apiKey);
}
