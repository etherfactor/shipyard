using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class AclUserEntryConfiguration : IEntityTypeConfiguration<AclUserEntry>
{
    public void Configure(
        EntityTypeBuilder<AclUserEntry> entity)
    {
        entity.ToView("user_entries", schema: "acl");

        entity.HasKey(e => e.EntryId);

        entity.Property(e => e.EntryId)
            .HasColumnName("entry_id");

        entity.Property(e => e.PrincipalUserId)
            .HasColumnName("principal_user_id");

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
