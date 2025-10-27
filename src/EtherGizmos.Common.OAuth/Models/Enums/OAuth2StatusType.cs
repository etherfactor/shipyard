using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Models.Enums;

public enum OAuth2StatusType
{
    Unknown = 0,
    Valid = 10,
    Inactive = 20,
    Redeemed = 30,
    Rejected = 40,
    Revoked = 50,
}

public static class OAuth2StatusTypeConverter
{
    private static Dictionary<OAuth2StatusType, string?> Mappings { get; } = new()
    {
        [OAuth2StatusType.Valid] = Statuses.Valid,
        [OAuth2StatusType.Inactive] = Statuses.Inactive,
        [OAuth2StatusType.Redeemed] = Statuses.Redeemed,
        [OAuth2StatusType.Rejected] = Statuses.Rejected,
        [OAuth2StatusType.Revoked] = Statuses.Revoked,
    };

    public static OAuth2StatusType FromString(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return OAuth2StatusType.Unknown;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static OAuth2StatusType? FromStringOrDefault(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string? ToString(
        OAuth2StatusType value)
    {
        if (value == OAuth2StatusType.Unknown)
            return "";

        if (!Mappings.ContainsKey(value))
            return "";

        return Mappings[value];
    }

    public static string? ToStringOrDefault(
        OAuth2StatusType? value)
    {
        if (value is null || value == OAuth2StatusType.Unknown)
            return null;

        if (!Mappings.ContainsKey(value.Value))
            return null;

        return Mappings[value.Value];
    }
}
