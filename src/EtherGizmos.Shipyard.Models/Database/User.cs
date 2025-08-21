using EtherGizmos.Common.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace EtherGizmos.Shipyard.Database;

public class User : InternalUser<Guid>, IEntity, IAuditable
{
    public DateTimeOffset? CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    public Guid? ModifiedByUserId { get; set; }

    public string Password { set => PasswordHash = _hasher.HashPassword(this, value); }

    private static readonly IPasswordHasher<User> _hasher = new PasswordHasher<User>();
}
