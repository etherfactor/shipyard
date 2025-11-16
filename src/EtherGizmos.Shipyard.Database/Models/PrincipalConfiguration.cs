using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class PrincipalConfiguration : IEntityTypeConfiguration<Principal>
{
    public void Configure(
        EntityTypeBuilder<Principal> entity)
    {
        entity.ToTable("principals", schema: "acl", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("principal_id")
            .HasDefaultValueSql();

        entity.Property(e => e.Type)
            .HasColumnName("principal_type_id");

        entity.HasMany(e => e.AclEntries)
            .WithOne()
            .HasForeignKey(e => e.PrincipalId);
    }
}
