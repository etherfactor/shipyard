using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class AclUserConfiguration : IEntityTypeConfiguration<AclUser>
{
    public void Configure(
        EntityTypeBuilder<AclUser> entity)
    {
        entity.ToView("users", schema: "acl");

        entity.HasKey(e => new { e.PrincipalUserId, e.UserId, e.PermissionId });

        entity.Property(e => e.PrincipalUserId)
            .HasColumnName("principal_user_id");

        entity.Property(e => e.UserId)
            .HasColumnName("user_id");

        entity.Property(e => e.PermissionId)
            .HasColumnName("permission_id");

        entity.Property(e => e.IsGrant)
            .HasColumnName("is_grant");

        entity.Property(e => e.PermissionGrantType)
            .HasColumnName("permission_grant_type_id");
    }
}
