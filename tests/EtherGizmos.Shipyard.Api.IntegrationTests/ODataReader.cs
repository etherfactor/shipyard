using System.Text.Json;

namespace EtherGizmos.Shipyard.Api.IntegrationTests;

public static class ODataReader
{
    public static async Task<(JsonDocument Root, IReadOnlyList<JsonElement> Items, int? Count)>
        ReadListRawAsync(HttpResponseMessage resp)
    {
        using var stream = await resp.Content.ReadAsStreamAsync();
        var doc = await JsonDocument.ParseAsync(stream);
        var root = doc.RootElement;
        var value = root.GetProperty("value");
        var items = value.EnumerateArray().ToList();
        int? count = root.TryGetProperty("@odata.count", out var c) ? c.GetInt32() : (int?)null;
        // NOTE: Return the doc so caller disposes when done
        return (doc, items, count);
    }

    public static async Task<(JsonDocument Root, JsonElement Item)>
        ReadSingleRawAsync(HttpResponseMessage resp)
    {
        using var stream = await resp.Content.ReadAsStreamAsync();
        var doc = await JsonDocument.ParseAsync(stream);
        return (doc, doc.RootElement);
    }
}
