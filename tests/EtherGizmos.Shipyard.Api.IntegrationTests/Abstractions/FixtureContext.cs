using EtherGizmos.Shipyard.Api.IntegrationTests.Controllers;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;

public record FixtureContext(Func<HttpClient> AnonymousClientFactory, ITokenMinter Minter)
{
    public static FixtureContext Instance { get; }

    static FixtureContext()
    {
        Instance = new(() => Setup.Client, new JwtTokenMinter());
    }

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
    public static HttpClient GetClientWithCapabilities(
        this FixtureContext @this,
        string subject,
        string capabilities = "Carrier:7;Package:7;User:7;Role:7;Group:7")
    {
        return @this.GetClient(new(subject)
        {
            Issuer = "http://localhost",
            Claims = new Dictionary<string, string>()
            {
                ["cap"] = capabilities,
                [Claims.Username] = "admin",
            },
        });
    }
}
