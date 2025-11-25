using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class AclEntryConfiguration : IEntityTypeConfiguration<AclEntry>
{
    public void Configure(
        EntityTypeBuilder<AclEntry> entity)
    {
        entity.ToTable("entries", schema: "acl", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("entry_id");

        entity.Property(e => e.PrincipalId)
            .HasColumnName("principal_id");

        entity.Property(e => e.SecurableId)
            .HasColumnName("securable_id");

        entity.Property(e => e.SecurableType)
            .HasColumnName("securable_type_id");

        entity.Property(e => e.PermissionId)
            .HasColumnName("permission_id");

        entity.Property(e => e.PermissionGrantType)
            .HasColumnName("permission_grant_type_id");
    }
}
