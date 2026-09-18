namespace EtherGizmos.Shipyard.Abstractions;

public interface ITokenMinter
{
    string Mint(TokenRequest request);
}
