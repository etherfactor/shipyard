using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class AclCarrierConfiguration : IEntityTypeConfiguration<AclCarrier>
{
    public void Configure(
        EntityTypeBuilder<AclCarrier> entity)
    {
        entity.ToView("carriers", schema: "acl");

        entity.HasKey(e => new { e.PrincipalUserId, e.CarrierId, e.PermissionId });

        entity.Property(e => e.PrincipalUserId)
            .HasColumnName("principal_user_id");

        entity.Property(e => e.CarrierId)
            .HasColumnName("carrier_id");

        entity.Property(e => e.PermissionId)
            .HasColumnName("permission_id");

        entity.Property(e => e.IsGrant)
            .HasColumnName("is_grant");

        entity.Property(e => e.PermissionGrantType)
            .HasColumnName("permission_grant_type_id");
    }
}
