using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(
        EntityTypeBuilder<Role> entity)
    {
        entity.ToTable("roles", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("role_id");

        entity.AuditProperties();

        entity.Property(e => e.Name)
            .HasColumnName("name");

        entity.Property(e => e.Description)
            .HasColumnName("description");

        entity.Property(e => e.SystemId)
            .HasColumnName("system_id");

        entity.HasMany(e => e.Users)
            .WithMany(e => e.Roles)
            .UsingEntity<RoleUser>();

        entity.Property(e => e.PrincipalId)
            .HasColumnName("principal_id");

        entity.HasOne(e => e.Principal);

        entity.Property(e => e.SecurableId)
            .HasColumnName("securable_id");
    }
}
