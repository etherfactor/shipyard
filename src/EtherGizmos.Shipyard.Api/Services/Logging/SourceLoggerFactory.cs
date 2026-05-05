using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
using System.Collections.Concurrent;

namespace EtherGizmos.Shipyard.Services.Logging;

internal class SourceLoggerFactory : ISourceLoggerFactory
{
    private readonly IOptions<LogIngestionOptions> _logOptions;
    private readonly IConfiguration _configuration;
    private readonly ConcurrentDictionary<string, Serilog.ILogger> _loggers = [];

    public SourceLoggerFactory(
        IOptions<LogIngestionOptions> logOptions,
        IConfiguration configuration)
    {
        _logOptions = logOptions;
        _configuration = configuration;
    }

    public Serilog.ILogger GetLogger(string apiKey)
    {
        var options = _logOptions.Value;

        var source = options.Sources.SingleOrDefault(e => e.Value.ApiKey.Equals(apiKey, StringComparison.OrdinalIgnoreCase));
        if (source.Key is not null)
        {
            var logger = _loggers.GetOrAdd(source.Value.ApiKey, apiKey =>
            {
                var sharedRoot = _configuration.GetSection("LogIngestion");
                var sharedSerilog = sharedRoot.GetSection("Serilog");

                var sourceRoot = sharedRoot.GetSection("Sources").GetSection(source.Key);
                var sourceSerilog = sourceRoot.GetSection("Serilog");

                var logger = new LoggerConfiguration();

                if (sourceSerilog.Exists())
                {
                    logger.ReadFrom.Configuration(sourceRoot);
                }
                else if (sharedSerilog.Exists())
                {
                    logger.ReadFrom.Configuration(sharedRoot);
                }
                else
                {
                    logger.ReadFrom.Configuration(_configuration);
                }

                return logger.CreateLogger();
            });

            return logger;
        }

        return Logger.None;
    }
}
