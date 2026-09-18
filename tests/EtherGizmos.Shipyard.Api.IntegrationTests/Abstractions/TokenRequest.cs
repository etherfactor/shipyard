namespace EtherGizmos.Shipyard.Abstractions;

public record TokenRequest(string Subject)
{
    public IReadOnlyDictionary<string, string>? Claims { get; init; } = null;

    public DateTimeOffset? Expires { get; init; } = null;

    public string? Audience { get; init; } = null;

    public string? Issuer { get; init; } = null;
}
