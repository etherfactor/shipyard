using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(
        EntityTypeBuilder<Group> entity)
    {
        entity.ToTable("groups", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("group_id");

        entity.AuditProperties();

        entity.Property(e => e.Name)
            .HasColumnName("name");

        entity.Property(e => e.Description)
            .HasColumnName("description");

        entity.Property(e => e.SystemId)
            .HasColumnName("system_id");

        entity.Property(e => e.SecurableId)
            .HasColumnName("securable_id");

        entity.HasOne(e => e.Securable);
    }
}
