using Asp.Versioning;
using EtherGizmos.Shipyard.Api.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using Serilog.Parsing;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ExceptionServices;
using System.Text.Json;

namespace EtherGizmos.Shipyard.Api.Controllers;

[ApiController]
public class LogsController : ControllerBase
{
    private const string BaseRoute = "api/v{version:apiVersion}/logs";

    private readonly ISourceLoggerFactory _sourceLoggerFactory;

    public LogsController(
        ISourceLoggerFactory sourceLoggerFactory)
    {
        _sourceLoggerFactory = sourceLoggerFactory;
    }

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute)]
    public IActionResult Ingest(
        [FromBody] List<LogEntry> logEntries,
        [FromQuery] string apiKey,
        CancellationToken cancellationToken = default)
    {
        var logger = _sourceLoggerFactory.GetLogger(apiKey);
        var serilogEntries = logEntries.Select(CreateLogEvent);

        foreach (var log in serilogEntries)
        {
            logger.Write(log);
        }

        return NoContent();
    }

    private LogEvent CreateLogEvent(
        LogEntry log)
    {
        var logLevel = log.Severity switch
        {
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            LogSeverity.Information => LogEventLevel.Information,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Fatal => LogEventLevel.Fatal,
            _ => LogEventLevel.Information,
        };

        var messageTemplate = new MessageTemplateParser().Parse(log.Message);

        var properties = new List<LogEventProperty>();

        if (!string.IsNullOrWhiteSpace(log.SourceContext))
        {
            properties.Add(new("SourceContext", new ScalarValue(log.SourceContext)));
        }

        Exception? exception = null;
        if (log.Exception is not null)
        {
            exception = new Exception(log.Exception.Message);

            if (log.Exception.StackTrace is not null)
            {
                exception = ExceptionDispatchInfo.SetRemoteStackTrace(exception, log.Exception.StackTrace);
            }
        }

        if (log.Properties.HasValue)
        {
            foreach (var property in log.Properties.Value.EnumerateObject())
            {
                properties.Add(new(property.Name, ConvertJson(property.Value)));
            }
        }

        var localTz = TimeZoneInfo.Local;
        var timestamp = TimeZoneInfo.ConvertTime(log.Timestamp!.Value, localTz);

        var logEvent = new LogEvent(
            timestamp,
            logLevel,
            exception,
            messageTemplate,
            properties);

        return logEvent;
    }

    private LogEventPropertyValue ConvertJson(
        JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => ConvertJsonObject(element),
            JsonValueKind.Array => ConvertJsonArray(element),
            JsonValueKind.String => new ScalarValue(element.GetString()),
            JsonValueKind.Number =>
                element.TryGetInt64(out long l)
                    ? new ScalarValue(l)
                    : new ScalarValue(element.GetDecimal()),
            JsonValueKind.True => new ScalarValue(element.GetBoolean()),
            JsonValueKind.False => new ScalarValue(element.GetBoolean()),
            JsonValueKind.Null => new ScalarValue(null),
            _ => new ScalarValue(null),
        };
    }

    private LogEventPropertyValue ConvertJsonArray(
        JsonElement element)
    {
        var elements = new List<LogEventPropertyValue>();
        foreach (var jsonElement in element.EnumerateArray())
        {
            elements.Add(ConvertJson(jsonElement));
        }

        return new SequenceValue(elements);
    }

    private LogEventPropertyValue ConvertJsonObject(
        JsonElement element)
    {
        var properties = new List<LogEventProperty>();
        foreach (var property in element.EnumerateObject())
        {
            properties.Add(new(property.Name, ConvertJson(property.Value)));
        }

        return new StructureValue(properties);
    }

    public enum LogSeverity
    {
        Verbose,
        Debug,
        Information,
        Warning,
        Error,
        Fatal,
    }

    public class LogEntry
    {
        [Required]
        public DateTimeOffset? Timestamp { get; set; }

        [Required]
        public LogSeverity? Severity { get; set; }

        public string? SourceContext { get; set; }

        [Required]
        public string Message { get; set; } = null!;

        public LogException? Exception { get; set; }

        public JsonElement? Properties { get; set; }
    }

    public class LogException
    {
        [Required]
        public string Message { get; set; } = null!;

        public string? StackTrace { get; set; }
    }
}
