using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Models.Enums;

public enum OAuth2TokenType
{
    Unknown = 0,
    Bearer = 10,
    AccessToken = 20,
    IdentityToken = 21,
    RefreshToken = 22,
    StateToken = 30,
    AuthorizationCode = 40,
}
public static class OAuth2TokenTypeConverter
{
    private static Dictionary<OAuth2TokenType, string?> Mappings { get; } = new()
    {
        [OAuth2TokenType.Bearer] = TokenTypes.Bearer,
        [OAuth2TokenType.AccessToken] = "urn:ietf:params:oauth:token-type:access_token",
        [OAuth2TokenType.IdentityToken] = "urn:ietf:params:oauth:token-type:id_token",
        [OAuth2TokenType.RefreshToken] = "urn:ietf:params:oauth:token-type:refresh_token",
        [OAuth2TokenType.StateToken] = "urn:openiddict:params:oauth:token-type:state_token",
        [OAuth2TokenType.AuthorizationCode] = "urn:openiddict:params:oauth:token-type:authorization_code",
    };

    public static OAuth2TokenType FromString(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return OAuth2TokenType.Unknown;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static OAuth2TokenType? FromStringOrDefault(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string? ToString(
        OAuth2TokenType value)
    {
        if (value == OAuth2TokenType.Unknown)
            return "";

        if (!Mappings.ContainsKey(value))
            return "";

        return Mappings[value];
    }

    public static string? ToStringOrDefault(
        OAuth2TokenType? value)
    {
        if (value is null || value == OAuth2TokenType.Unknown)
            return null;

        if (!Mappings.ContainsKey(value.Value))
            return null;

        return Mappings[value.Value];
    }
}
