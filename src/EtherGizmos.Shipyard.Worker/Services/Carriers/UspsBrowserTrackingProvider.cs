using EtherGizmos.Shipyard.Models.Database.Enums;
using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Web;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

internal class UspsBrowserTrackingProvider : ITrackingProvider
{
    private readonly IBrowserClient _client;

    public UspsBrowserTrackingProvider(
        IServiceProvider serviceProvider)
    {
        _client = serviceProvider.GetRequiredService<IBrowserClient>();
    }

    public async Task<TrackingResult> TrackAsync(
        string trackingNumber,
        CancellationToken cancellationToken = default)
    {
        await _client.NavigateAsync($"https://tools.usps.com/go/TrackAction?tLabels={HttpUtility.UrlEncode(trackingNumber)}", cancellationToken: cancellationToken);
        await _client.WaitForElementAsync("span.tracking-number", cancellationToken: cancellationToken);
        await _client.ClickElementAsync("div.toggle-history-container", cancellationToken: cancellationToken);

        var html = await _client.GetHtmlAsync(cancellationToken);

        var statusRegex = new Regex(@"<p class=""tb-status-detail"">\s*(?<detail>[^<]*?)\s*</p>(?:\s*<p class=""tb-location"">\s*(?<location>[^<]*?)\s*</p>)?\s*<p class=""tb-date"">\s*(?<date>[^<]*?)\s*</p>");
        var statusMatches = statusRegex.Matches(html);

        var statuses = statusMatches
            .Select(match =>
            {
                var detail = match.Groups["detail"].Value;
                var location = match.Groups["location"].Value;
                var date = match.Groups["date"].Value;

                return new TrackingResultDetail
                {
                    EventOccurredAt = DateTime.Parse(date),
                    StatusTypeId = StatusTypeId.InTransit,
                    Location = NullIfEmpty(location),
                    Description = NullIfEmpty(detail),
                };
            })
            .OrderBy(e => e.EventOccurredAt)
            .ToImmutableList();

        DateTimeOffset? estimatedAt;

        var estimateRegex = new Regex(@"<strong class=""date"">\s*(?<day>[^>]*?)\s*</strong>\s*<span class=""month_year"">\s*<span>\s*(?<month>[^>]*?)\s*</span>\s*(?<year>[^<]*?)\s*<span.*?<strong class=""time"">\s*(?<time>[^<]*?)\s*<span", RegexOptions.Singleline);
        var estimateMatch = estimateRegex.Matches(html).FirstOrDefault();
        if (estimateMatch is not null)
        {
            var year = estimateMatch.Groups["year"].Value;
            var month = estimateMatch.Groups["month"].Value;
            var day = estimateMatch.Groups["day"].Value;
            var time = estimateMatch.Groups["time"].Value;

            estimatedAt = DateTime.Parse($"{month} {day}, {year} {time}");
        }
        else
        {
            estimatedAt = null;
        }

        var result = new TrackingResult
        {
            TrackingNumber = trackingNumber,
            EstimatedDeliveryAt = estimatedAt,
            Details = statuses,
        };

        return result;
    }

    private string? NullIfEmpty(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? input : null;
    }
}
