using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class AclOrderConfiguration : IEntityTypeConfiguration<AclOrder>
{
    public void Configure(
        EntityTypeBuilder<AclOrder> entity)
    {
        entity.ToView("orders", schema: "acl");

        entity.HasKey(e => new { e.PrincipalUserId, e.OrderId, e.PermissionId });

        entity.Property(e => e.PrincipalUserId)
            .HasColumnName("principal_user_id");

        entity.Property(e => e.OrderId)
            .HasColumnName("order_id");

        entity.Property(e => e.PermissionId)
            .HasColumnName("permission_id");

        entity.Property(e => e.IsGrant)
            .HasColumnName("is_grant");

        entity.Property(e => e.PermissionGrantType)
            .HasColumnName("permission_grant_type_id");
    }
}
