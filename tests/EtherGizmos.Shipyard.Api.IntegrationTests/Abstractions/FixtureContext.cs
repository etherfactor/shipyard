namespace EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;

public record FixtureContext(Func<HttpClient> AnonymousClientFactory, ITokenMinter Minter)
{
    public HttpClient GetClient(
        TokenRequest request)
    {
        var token = Minter.Mint(request);
        var client = AnonymousClientFactory();

        client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        return client;
    }

    public HttpClient GetAnonymousClient() =>
        AnonymousClientFactory();
}

public static class FixtureContextExtensions
{
    public static HttpClient GetClientAsRole(
        this FixtureContext @this,
        string subject,
        int roleId)
    {
        return @this.GetClient(new(subject)
        {
            Issuer = "http://localhost",
            Claims = new Dictionary<string, string>()
            {
                ["role"] = roleId.ToString(),
            },
        });
    }
}
