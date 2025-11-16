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

    public virtual int? GroupId { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual List<Role> Roles { get; set; } = [];

    public virtual Guid PrincipalId { get; set; }

    public virtual Principal Principal { get; set; } = new() { Type = Enums.PrincipalType.User };

    public virtual Guid SecurableId { get; set; }

    public virtual Securable Securable { get; set; } = new() { Type = Enums.SecurableType.User };
}
