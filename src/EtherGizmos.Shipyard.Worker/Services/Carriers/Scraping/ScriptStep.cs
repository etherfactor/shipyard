using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Services.WebDrivers;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Jint;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EtherGizmos.Shipyard.Services.Carriers.Scraping;

internal class ScriptStep : ScrapingStep
{
    public override string StepName => $"Execute user script ({Script.Split('\n').Length} lines)";

    [Required]
    public string Script { get; set; } = null!;

    public override async Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        var html = await client.GetHtmlAsync(cancellationToken);
        var result = ScriptTransform.Run(html, Script, ServiceProvider, Logger);

        if (result.EstimatedDeliveryAt is not null)
        {
            SetEta(result.EstimatedDeliveryAt.Value);
        }

        foreach (var @event in result.Details)
        {
            AddEvent(@event);
        }
    }

    protected internal override Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        return Apply(client, variables, results, cancellationToken);
    }

    public sealed record NodeHandle
    {
        public int Id { get; init; }

        public NodeHandle(int id) => Id = id;
    }

    public sealed class HapSnapshot
    {
        public HtmlDocument Document { get; init; }

        public IReadOnlyDictionary<int, HtmlNode> IdToNode { get; init; } = new Dictionary<int, HtmlNode>();

        public IReadOnlyDictionary<HtmlNode, int> NodeToId { get; init; } = new Dictionary<HtmlNode, int>();

        public HapSnapshot(HtmlDocument document) => Document = document;

        public static HapSnapshot FromHtml(
            string html,
            int maxNodes = 20_000)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            doc.OptionEmptyCollection = true;

            foreach (var n in doc.DocumentNode.SelectNodes("//script|//style|//comment()")!)
            {
                n.Remove();
            }

            var id = 0;
            var nodes = new Dictionary<int, HtmlNode>();
            void Walk(HtmlNode n)
            {
                if (nodes.Count >= maxNodes) return;
                nodes[++id] = n;
                foreach (var c in n.ChildNodes) Walk(c);
            }
            Walk(doc.DocumentNode);

            var snap = new HapSnapshot(doc)
            {
                IdToNode = nodes,
                NodeToId = nodes.ToDictionary(e => e.Value, e => e.Key),
            };

            return snap;
        }
    }

    public sealed class HapHost
    {
        private readonly HapSnapshot _snapshot;
        private readonly ILogger _logger;
        private readonly int _maxResults;
        private readonly int _maxCalls;
        private int _calls;

        public HapHost(
            HapSnapshot snapshot,
            ILogger logger,
            int maxResults = 1000,
            int maxCalls = 2000)
        {
            _snapshot = snapshot;
            _logger = logger;
            _maxResults = maxResults;
            _maxCalls = maxCalls;
        }

        public NodeHandle[] SelectAll(string selector)
            => QAllCssInner(_snapshot.Document.DocumentNode, selector);

        public NodeHandle[] SubQAllCss(NodeHandle h, string selector)
            => QAllCssInner(NodeOf(h), selector);

        private NodeHandle[] QAllCssInner(HtmlNode n, string selector)
            => Limit(n.QuerySelectorAll(selector).Select(HandleOf));

        public string Text(NodeHandle h)
            => Normalize(NodeOf(h).InnerText);

        public string Html(NodeHandle h)
            => Normalize(NodeOf(h).InnerHtml);

        public bool HasAttribute(NodeHandle h, string name)
            => NodeOf(h).Attributes
                .FirstOrDefault(e => string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase)) is not null;

        public string Attribute(NodeHandle h, string name)
            => NodeOf(h).Attributes[name]?.Value ?? "";

        public IReadOnlyDictionary<string, string> Attributes(NodeHandle h)
            => NodeOf(h).Attributes
                .ToDictionary(e => e.Name.ToLowerInvariant(), e => e.Value ?? "");

        public bool HasClass(NodeHandle h, string name)
            => NodeOf(h).HasClass(name);

        public string[] Classes(NodeHandle h)
            => [.. NodeOf(h).GetClasses()];

        public NodeHandle Parent(NodeHandle h)
            => HandleOf(NodeOf(h).ParentNode);

        private NodeHandle HandleOf(HtmlNode n)
            => new(_snapshot.NodeToId[n]);

        private HtmlNode NodeOf(NodeHandle h)
            => _snapshot.IdToNode[h.Id];

        private TValue[] Limit<TValue>(IEnumerable<TValue> seq)
        {
            //CheckBudget();
            return [.. seq.Take(_maxResults)];
        }

        private static string Normalize(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            input = HtmlEntity.DeEntitize(input)?.Replace('\u00A0', ' ') ?? "";
            return Regex.Replace(input, @"\s+", " ").Trim();
        }
    }

    public static class ScriptTransform
    {
        public static TrackingResult Run(string html, string js, IServiceProvider serviceProvider, ILogger logger)
        {
            var snapshot = HapSnapshot.FromHtml(html);
            var host = new HapHost(snapshot, logger);

            DateTimeOffset? eta = null;
            var events = new List<TrackingResultDetail>();
            var engine = new Engine(o => o.Strict().LimitMemory(16_000_000).TimeoutInterval(TimeSpan.FromSeconds(600)));

            var nodeProto = engine.Intrinsics.Object.Construct(Arguments.Empty);

            JsValue Box(NodeHandle handle)
            {
                var o = engine.Intrinsics.Object.Construct(Arguments.Empty);
                o.Set("_hid", JsValue.FromObject(engine, handle.Id), true);
                o.Prototype = nodeProto;
                return o;
            }

            NodeHandle Unbox(JsValue value)
            {
                var id = (int)value.AsObject().Get("_hid").AsNumber();
                return new(id);
            }

            JsValue BoxMany(IEnumerable<NodeHandle> handles) => engine.Intrinsics.Array.ConstructFast([.. handles.Select(Box)]);

            string JoinArgs(JsValue[] args) => string.Join(" ", args.Select(a => a.IsString() ? a.AsString() : a.ToString()));

            nodeProto.Set("text", new ClrFunction(engine, "text", (thisObj, args) =>
            {
                var handle = Unbox(thisObj);
                return JsValue.FromObject(engine, host.Text(handle));
            }), true);

            nodeProto.Set("html", new ClrFunction(engine, "html", (thisObj, args) =>
            {
                var handle = Unbox(thisObj);
                return JsValue.FromObject(engine, host.Html(handle));
            }), true);

            nodeProto.Set("hasAttribute", new ClrFunction(engine, "hasAttribute", (thisObj, args) =>
            {
                var handle = Unbox(thisObj);
                var name = args.At(0).AsString();
                return JsValue.FromObject(engine, host.HasAttribute(handle, name));
            }), true);

            nodeProto.Set("attribute", new ClrFunction(engine, "attribute", (thisObj, args) =>
            {
                var handle = Unbox(thisObj);
                var name = args.At(0).AsString();
                return JsValue.FromObject(engine, host.Attribute(handle, name));
            }), true);

            nodeProto.Set("attributes", new ClrFunction(engine, "attributes", (thisObj, args) =>
            {
                var handle = Unbox(thisObj);
                var attributes = host.Attributes(handle);
                var obj = engine.Intrinsics.Object.Construct(Arguments.Empty);
                foreach (var (key, value) in attributes)
                    obj.DefineOwnProperty(JsValue.FromObject(engine, key), new PropertyDescriptor(JsValue.FromObject(engine, value), writable: false, enumerable: true, configurable: false));
                return obj;
            }), true);

            nodeProto.Set("hasClass", new ClrFunction(engine, "hasClass", (thisObj, args) =>
            {
                var handle = Unbox(thisObj);
                var name = args.At(0).AsString();
                return JsValue.FromObject(engine, host.HasClass(handle, name));
            }), true);

            nodeProto.Set("classes", new ClrFunction(engine, "classes", (thisObj, args) =>
            {
                var handle = Unbox(thisObj);
                return JsValue.FromObject(engine, host.Classes(handle));
            }), true);

            nodeProto.Set("parent", new ClrFunction(engine, "parent", (thisObj, args) =>
            {
                var handle = Unbox(thisObj);
                return Box(host.Parent(handle));
            }), true);

            nodeProto.Set("selectOne", new ClrFunction(engine, "selectOne", (thisObj, args) =>
            {
                var handle = Unbox(thisObj);
                var selector = args.At(0).AsString();
                var all = host.SubQAllCss(handle, selector);
                return all.Length == 0 ? JsValue.Null : Box(all[0]);
            }), true);

            nodeProto.Set("selectAll", new ClrFunction(engine, "selectAll", (thisObj, args) =>
            {
                var handle = Unbox(thisObj);
                var selector = args.At(0).AsString();
                return BoxMany(host.SelectAll(selector));
            }), true);

            engine.SetValue("selectOne", (Func<string, JsValue>)(selector =>
            {
                var all = host.SelectAll(selector);
                return all.Length == 0 ? JsValue.Null : Box(all[0]);
            }));

            engine.SetValue("selectAll", (Func<string, JsValue>)(selector => BoxMany(host.SelectAll(selector))));

            engine.SetValue("setEta", (Action<string>)(etaStr =>
            {
                eta = DateTimeOffset.Parse(etaStr);
            }));

            engine.SetValue("recordEvent", (Action<JsValue>)(o =>
            {
                var obj = o.AsObject();
                var when = DateTimeOffset.Parse(obj.Get("at").AsString());
                var result = new TrackingResultDetail
                {
                    StatusTypeId = 0,
                    OccurredAt = when,
                    Description = obj.Get("description").AsString() ?? "",
                    Location = obj.Get("location").IsUndefined() ? null : obj.Get("location").AsString(),
                };

                events.Add(result with
                {
                    Description = result.Description.Length > 2000 ? result.Description[..2000] : result.Description,
                    Location = result.Location?.Length > 500 ? result.Location[..500] : result.Location,
                });
            }));

            engine.SetValue("normalizeLocalDate", (Func<int, int, int, int, int, int, string?, JsValue>)((a, b, c, d, e, f, g) =>
            {
                var result = NormalizeLocalDate(serviceProvider, a, b, c, d, e, f, g);
                return JsValue.FromObject(engine, result);
            }));

            engine.Execute("""
                function normalizeDateString(input) {
                    if (!input) return input;

                    let s = input.trim();

                    //Collapse whitespace
                    s = s.replace(/\s+/g, " ");

                    //Normalize A.M. / P.M. → AM / PM
                    s = s.replace(/A\.M\./gi, "AM")
                         .replace(/P\.M\./gi, "PM");

                    return s;
                }

                function parseDate(input, location) {
                    const normalized = normalizeDateString(input);
                    const date = new Date(normalized);
                    console.log("Produced date", date, "from", input, "with ISO", date.toISOString());

                    if (isNaN(date.getTime())) {
                        throw new Error("Could not parse date: " + input);
                    }

                    const year = date.getFullYear();
                    const month = date.getMonth() + 1; //For whatever reason, months are 0-indexed
                    const day = date.getDate();
                    const hour = date.getHours();
                    const minute = date.getMinutes();
                    const second = date.getSeconds();

                    return normalizeLocalDate(year, month, day, hour, minute, second, location ?? null);
                }
                """);

            var console = engine.Intrinsics.Object.Construct(Arguments.Empty);

            console.Set("log", new ClrFunction(engine, "log", (thisObj, args) =>
            {
                using var js = logger.BeginScope("Language", "JavaScript");
                logger.LogInformation("[js] {Msg}", JoinArgs(args));
                return JsValue.Undefined;
            }), true);

            console.Set("warn", new ClrFunction(engine, "warn", (thisObj, args) =>
            {
                using var js = logger.BeginScope("Language", "JavaScript");
                logger.LogWarning("[js] {Msg}", JoinArgs(args));
                return JsValue.Undefined;
            }), true);

            console.Set("error", new ClrFunction(engine, "error", (thisObj, args) =>
            {
                using var js = logger.BeginScope("Language", "JavaScript");
                logger.LogError("[js] {Msg}", JoinArgs(args));
                return JsValue.Undefined;
            }), true);

            engine.SetValue("console", console);

            engine.Execute(js);

            return new()
            {
                TrackingNumber = "",
                EstimatedDeliveryAt = eta,
                Details = events,
                Artifacts = [],
            };
        }
    }

    private static string NormalizeLocalDate(
        IServiceProvider serviceProvider,
        int year,
        int month,
        int day,
        int hour,
        int minute,
        int second,
        string? location)
    {
        var options = serviceProvider
            .GetRequiredService<IOptionsMonitor<WorkerOptions>>()
            .CurrentValue;

        // 1. Interpret this as a wall-clock time in the *target* time zone
        var localWallClock = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);

        var targetTz = ResolveTimeZone(serviceProvider, location); // e.g. DefaultTimeZone = "America/Chicago"

        // 2. Convert FROM target time zone TO UTC
        var utc = TimeZoneInfo.ConvertTimeToUtc(localWallClock, targetTz);

        // 3. Return an ISO 8601 UTC string
        var dateString = utc.ToString("O");
        return dateString;
    }

    private static TimeZoneInfo ResolveTimeZone(
        IServiceProvider serviceProvider,
        string? location)
    {
        var options = serviceProvider
            .GetRequiredService<IOptionsMonitor<WorkerOptions>>()
            .CurrentValue;

        return TimeZoneInfo.FindSystemTimeZoneById(options.DefaultTimeZone);
    }
}
