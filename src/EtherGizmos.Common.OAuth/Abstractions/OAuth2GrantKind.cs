using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Abstractions;

public enum OAuth2GrantKind
{
    Unknown = 0,
    AuthorizationCode = 1,
    RefreshToken = 2,
    ClientCredentials = 3,
    Password = 4,
    DeviceCode = 5,
    TokenExchange = 6,
}

public static class OAuth2GrantKindConverter
{
    private static Dictionary<OAuth2GrantKind, string?> Mappings { get; } = new()
    {
        [OAuth2GrantKind.AuthorizationCode] = GrantTypes.AuthorizationCode,
        [OAuth2GrantKind.RefreshToken] = GrantTypes.RefreshToken,
        [OAuth2GrantKind.ClientCredentials] = GrantTypes.ClientCredentials,
        [OAuth2GrantKind.Password] = GrantTypes.Password,
        [OAuth2GrantKind.DeviceCode] = GrantTypes.DeviceCode,
        [OAuth2GrantKind.TokenExchange] = GrantTypes.TokenExchange,
    };

    public static OAuth2GrantKind FromString(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return OAuth2GrantKind.Unknown;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static OAuth2GrantKind? FromStringOrDefault(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string? ToString(
        OAuth2GrantKind value)
    {
        if (value == OAuth2GrantKind.Unknown)
            return "";

        if (!Mappings.ContainsKey(value))
            return "";

        return Mappings[value];
    }

    public static string? ToStringOrDefault(
        OAuth2GrantKind? value)
    {
        if (value is null || value == OAuth2GrantKind.Unknown)
            return null;

        if (!Mappings.ContainsKey(value.Value))
            return null;

        return Mappings[value.Value];
    }
}
