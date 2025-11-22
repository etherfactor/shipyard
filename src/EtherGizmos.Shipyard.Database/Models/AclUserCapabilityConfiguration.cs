using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class AclUserCapabilityConfiguration : IEntityTypeConfiguration<AclUserCapability>
{
    public void Configure(
        EntityTypeBuilder<AclUserCapability> entity)
    {
        entity.ToTable("user_capabilities", schema: "acl");

        entity.HasKey(e => new { e.PrincipalUserId, e.SecurableType, e.PermissionId });

        entity.Property(e => e.PrincipalUserId)
            .HasColumnName("principal_user_id");

        entity.Property(e => e.SecurableType)
            .HasColumnName("securable_type_id");

        entity.Property(e => e.PermissionId)
            .HasColumnName("permission_id");

        entity.Property(e => e.IsAllowed)
            .HasColumnName("is_allowed");
    }
}
