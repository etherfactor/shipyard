using Microsoft.Extensions.Logging;
using Serilog.Events;
using Serilog.Parsing;
using System.Collections;
using System.Globalization;

namespace EtherGizmos.Common.Services;

internal class TeeStreamLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new TeeLogger(categoryName);
    public void Dispose() { /* nothing */ }

    private sealed class TeeLogger : ILogger
    {
        private readonly string _category;
        public TeeLogger(string category) => _category = category;

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true; // cheap check in Log()

        public void Log<TState>(LogLevel level, EventId id, TState state, Exception? ex,
            Func<TState, Exception?, string> formatter)
        {
            var ctx = TeeStreamAmbient.Current;
            if (ctx is null) return; // tee is inactive → do nothing

            // Minimal work when inactive; when active, format as Serilog LogEvent
            var serilogLevel = level switch
            {
                LogLevel.Trace => LogEventLevel.Verbose,
                LogLevel.Debug => LogEventLevel.Debug,
                LogLevel.Information => LogEventLevel.Information,
                LogLevel.Warning => LogEventLevel.Warning,
                LogLevel.Error => LogEventLevel.Error,
                LogLevel.Critical => LogEventLevel.Fatal,
                _ => LogEventLevel.Information
            };

            var properties = new List<LogEventProperty>
            {
                new("CategoryName", new ScalarValue(_category)),
                new("EventId", new ScalarValue(id.Id)),
            };

            if (state is IEnumerable<KeyValuePair<string, object>> kvs)
            {
                foreach (var kv in kvs)
                {
                    // avoid overwriting built-ins like "{OriginalFormat}" if you don’t want them
                    if (kv.Key is not null)
                        properties.Add(new LogEventProperty(kv.Key, ToSerilogValue(kv.Value)));
                }
            }

            var message = formatter(state, ex);
            var evt = new LogEvent(
                DateTimeOffset.UtcNow,
                serilogLevel,
                ex,
                new MessageTemplate(message, new List<MessageTemplateToken>()),
                properties);

            ctx.Formatter.Format(evt, ctx.Writer);
        }

        static LogEventPropertyValue ToSerilogValue(object? value)
        {
            if (value is null) return new ScalarValue(null);

            // Common primitives first
            switch (value)
            {
                case string s: return new ScalarValue(s);
                case bool b: return new ScalarValue(b);
                case byte or sbyte or short or ushort or int or uint or long or ulong:
                    return new ScalarValue(Convert.ToInt64(value, CultureInfo.InvariantCulture));
                case float or double or decimal:
                    return new ScalarValue(Convert.ToDecimal(value, CultureInfo.InvariantCulture));
                case DateTime dt: return new ScalarValue(dt);
                case DateTimeOffset dto: return new ScalarValue(dto);
                case Guid g: return new ScalarValue(g);
                case Uri u: return new ScalarValue(u.ToString());
            }

            // KeyValue pairs → Structure
            if (value is IEnumerable<KeyValuePair<string, object?>> objKvp)
            {
                var props = new List<LogEventProperty>();
                foreach (var kv in objKvp)
                    props.Add(new LogEventProperty(kv.Key, ToSerilogValue(kv.Value)));
                return new StructureValue(props);
            }

            // Any non-string IEnumerable → Sequence
            if (value is IEnumerable seq && value is not string)
            {
                var items = new List<LogEventPropertyValue>();
                foreach (var item in seq) items.Add(ToSerilogValue(item));
                return new SequenceValue(items);
            }

            // Fallback: treat as scalar (serialized by formatter)
            return new ScalarValue(value);
        }

        private sealed class NullScope : IDisposable { public static readonly NullScope Instance = new(); public void Dispose() { } }
    }
}
