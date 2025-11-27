using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class SecurableConfiguration : IEntityTypeConfiguration<Securable>
{
    public void Configure(
        EntityTypeBuilder<Securable> entity)
    {
        entity.ToTable("securables", schema: "acl", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("securable_id")
            .HasDefaultValueSql();

        entity.Property(e => e.Type)
            .HasColumnName("securable_type_id");
    }
}
