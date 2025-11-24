using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class AclGroupConfiguration : IEntityTypeConfiguration<AclGroup>
{
    public void Configure(
        EntityTypeBuilder<AclGroup> entity)
    {
        entity.ToView("groups", schema: "acl");

        entity.HasKey(e => new { e.PrincipalUserId, e.GroupId, e.PermissionId });

        entity.Property(e => e.PrincipalUserId)
            .HasColumnName("principal_user_id");

        entity.Property(e => e.GroupId)
            .HasColumnName("group_id");

        entity.Property(e => e.PermissionId)
            .HasColumnName("permission_id");

        entity.Property(e => e.IsGrant)
            .HasColumnName("is_grant");

        entity.Property(e => e.PermissionGrantType)
            .HasColumnName("permission_grant_type_id");
    }
}
