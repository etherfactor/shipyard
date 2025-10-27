namespace EtherGizmos.Common.Abstractions;

public abstract class InternalUser<TId> : IInternalUser
{
    public virtual TId Id { get; set; } = default!;

    public virtual string Username { get; set; } = null!;

    public virtual string? EmailAddress { get; set; }

    public virtual string PasswordHash { get; set; } = null!;

    public virtual string? GivenName { get; set; }

    public virtual string? FamilyName { get; set; }

    public virtual string? FullName { get; set; }
}
