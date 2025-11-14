using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class AclPackageConfiguration : IEntityTypeConfiguration<AclPackage>
{
    public void Configure(
        EntityTypeBuilder<AclPackage> entity)
    {
        entity.ToView("packages", schema: "acl");

        entity.HasKey(e => new { e.PrincipalUserId, e.PackageId, e.PermissionId });

        entity.Property(e => e.PrincipalUserId)
            .HasColumnName("principal_user_id");

        entity.Property(e => e.PackageId)
            .HasColumnName("package_id");

        entity.Property(e => e.PermissionId)
            .HasColumnName("permission_id");

        entity.Property(e => e.IsGrant)
            .HasColumnName("is_grant");

        entity.Property(e => e.PermissionGrantType)
            .HasColumnName("permission_grant_type_id");
    }
}
