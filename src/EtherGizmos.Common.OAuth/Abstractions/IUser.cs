namespace EtherGizmos.Common.Abstractions;

public interface IUser
{
    string Username { get; set; }

    string? EmailAddress { get; set; }

    string? GivenName { get; set; }

    string? FamilyName { get; set; }

    string? FullName { get; set; }
}
