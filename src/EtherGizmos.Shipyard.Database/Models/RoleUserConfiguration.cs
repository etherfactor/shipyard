using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class RoleUserConfiguration : IEntityTypeConfiguration<RoleUser>
{
    public void Configure(
        EntityTypeBuilder<RoleUser> entity)
    {
        entity.ToTable("role_users", table => table.HasTrigger("TR"));

        entity.HasKey(e => new { e.RoleId, e.UserId });

        entity.Property(e => e.RoleId)
            .HasColumnName("role_id");

        entity.Property(e => e.UserId)
            .HasColumnName("user_id");

        entity.AuditProperties();
    }
}
