using EtherGizmos.Common.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace EtherGizmos.Shipyard.Database;

public class User : InternalUser<Guid>, IEntity, IAuditable
{
    public virtual DateTimeOffset? CreatedAt { get; set; }

    public virtual Guid? CreatedByUserId { get; set; }

    public virtual DateTimeOffset? ModifiedAt { get; set; }

    public virtual Guid? ModifiedByUserId { get; set; }

    public virtual string Password
    {
        get => throw new NotSupportedException();
        set => PasswordHash = _hasher.HashPassword(this, value);
    }

    private static readonly IPasswordHasher<User> _hasher = new PasswordHasher<User>();

    public virtual List<Role> Roles { get; set; } = [];

    public virtual Guid PrincipalId { get; init; }

    public virtual Principal Principal { get; init; } = new() { Type = Enums.PrincipalType.User };
}
