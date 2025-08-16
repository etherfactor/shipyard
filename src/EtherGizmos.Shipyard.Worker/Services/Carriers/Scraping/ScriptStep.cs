using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Jint;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Interop;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

internal class ScriptStep : ScrapingStep
{
    [Required]
    public string Script { get; set; } = null!;

    public override async Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        var html = await client.GetHtmlAsync(cancellationToken);
        var result = ScriptTransform.Run(html, Script, Logger);

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

    //public sealed record TrackingEvent
    //{
    //    public DateTimeOffset OccurredAt { get; init; }

    //    public string? Location { get; init; }

    //    [Required]
    //    public string Description { get; init; } = null!;
    //}

    //public sealed record TrackingResult
    //{
    //    public DateTimeOffset? EstimatedAt { get; set; }

    //    public IList<TrackingEvent> Events { get; set; } = [];
    //}

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
            ILogger logger)
        {
            _snapshot = snapshot;
            _logger = logger;
        }

        public NodeHandle[] QAllCss(string selector)
            => QAllCssInner(_snapshot.Document.DocumentNode, selector);

        public NodeHandle[] SubQAllCss(NodeHandle h, string selector)
            => QAllCssInner(NodeOf(h), selector);

        private NodeHandle[] QAllCssInner(HtmlNode n, string selector)
            => Limit(n.QuerySelectorAll(selector).Select(HandleOf));

        public string Text(NodeHandle h)
            => Normalize(NodeOf(h).InnerText);

        public string Attr(NodeHandle h, string name)
            => NodeOf(h).Attributes[name]?.Value ?? "";

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
        public static TrackingResult Run(string html, string js, ILogger logger)
        {
            var snapshot = HapSnapshot.FromHtml(html);
            var host = new HapHost(snapshot, logger);

            DateTimeOffset? eta = null;
            var events = new List<TrackingResultDetail>();
            var engine = new Engine(o => o.Strict().LimitMemory(16_000_000).TimeoutInterval(TimeSpan.FromSeconds(1)));

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

            nodeProto.Set("attr", new ClrFunction(engine, "attr", (thisObj, args) =>
            {
                var handle = Unbox(thisObj);
                var name = args.At(0).AsString();
                return JsValue.FromObject(engine, host.Attr(handle, name));
            }), true);

            nodeProto.Set("qAll", new ClrFunction(engine, "qAll", (thisObj, args) =>
            {
                var handle = Unbox(thisObj);
                var selector = args.At(0).AsString();
                return BoxMany(host.QAllCss(selector));
            }), true);

            engine.SetValue("qAll", (Func<string, JsValue>)(selector => BoxMany(host.QAllCss(selector))));

            engine.SetValue("qOne", (Func<string, JsValue>)(selector =>
            {
                var all = host.QAllCss(selector);
                return all.Length == 0 ? JsValue.Null : Box(all[0]);
            }));

            engine.SetValue("eta", (Action<string>)(etaStr =>
            {
                eta = DateTimeOffset.Parse(etaStr);
            }));

            engine.SetValue("addEvent", (Action<JsValue>)(o =>
            {
                var obj = o.AsObject();
                var when = DateTimeOffset.Parse(obj.Get("occurredAt").AsString());
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

            var console = engine.Intrinsics.Object.Construct(Arguments.Empty);

            console.Set("log", new ClrFunction(engine, "log", (thisObj, args) =>
            {
                logger.LogInformation("[js] {Msg}", JoinArgs(args));
                return JsValue.Undefined;
            }), true);

            console.Set("warn", new ClrFunction(engine, "warn", (thisObj, args) =>
            {
                logger.LogWarning("[js] {Msg}", JoinArgs(args));
                return JsValue.Undefined;
            }), true);

            console.Set("error", new ClrFunction(engine, "error", (thisObj, args) =>
            {
                logger.LogError("[js] {Msg}", JoinArgs(args));
                return JsValue.Undefined;
            }), true);

            engine.SetValue("console", console);

            return new()
            {
                TrackingNumber = "",
                EstimatedDeliveryAt = eta,
                Details = events,
            };
        }
    }
}
