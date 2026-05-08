using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Extensions;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace EtherGizmos.Shipyard.Services.Carriers;

internal class RegexClassifier : IRegexClassifier
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RegexClassifier(
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<int> ClassifyStatusAsync(
        int carrierId,
        string description,
        CancellationToken cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient("API");

        var response = await client.GetAsync(
            $"/api/v1/carriers({carrierId})",
            cancellationToken: cancellationToken);

        response.EnsureSuccessStatusCode();

        var carrier = (await response.Content.ReadFromJsonAsync<CarrierDTO>(
            JsonSerializerOptions.App,
            cancellationToken: cancellationToken))!;

        foreach (var rule in carrier.Rules.OrderBy(e => e.Priority))
        {
            var regex = new Regex(rule.Pattern);
            if (regex.IsMatch(description))
            {
                return (int)rule.StatusType;
            }
        }

        return StatusTypeId.Unknown;
    }
}
