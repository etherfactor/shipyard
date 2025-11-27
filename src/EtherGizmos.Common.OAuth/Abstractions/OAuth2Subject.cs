namespace EtherGizmos.Common.Abstractions;

public record OAuth2Subject(
    OAuth2SubjectKind Kind,
    string Value)
{
    public static OAuth2Subject Create(
        OAuth2SubjectKind kind, string value)
        => new(kind, value);
}
