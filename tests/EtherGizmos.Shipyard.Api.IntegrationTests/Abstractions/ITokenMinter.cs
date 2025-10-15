namespace EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;

public interface ITokenMinter
{
    string Mint(TokenRequest request);
}
