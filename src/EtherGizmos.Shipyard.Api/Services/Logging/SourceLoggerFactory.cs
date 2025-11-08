using EtherGizmos.Shipyard.Api.Abstractions;
using EtherGizmos.Shipyard.Api.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
using System.Collections.Concurrent;

namespace EtherGizmos.Shipyard.Api.Services.Logging;

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

        var source = options.Sources.SingleOrDefault(e => e.Value.ApiKey.Equals(apiKey, StringComparison.InvariantCultureIgnoreCase));
        if (source.Key is not null)
        {
            var logger = _loggers.GetOrAdd(source.Value.ApiKey, apiKey =>
            {
                var sourceRoot = _configuration.GetSection($"LogIngestion:Sources:{source.Key}");

                var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(sourceRoot)
                    .CreateLogger();

                return logger;
            });

            return logger;
        }

        return Logger.None;
    }
}
