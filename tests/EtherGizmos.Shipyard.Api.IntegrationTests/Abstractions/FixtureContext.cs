namespace EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;

public record FixtureContext
{
    public HttpClient Client { get; init; } = null!;

    public object[]? Key { get; init; }
}
