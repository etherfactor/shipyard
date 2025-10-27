using EtherGizmos.Shipyard.Database.Extensions;
using EtherGizmos.Shipyard.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Database.Models;

public class CarrierStatusRuleConfiguration : IEntityTypeConfiguration<CarrierStatusRule>
{
    public void Configure(
        EntityTypeBuilder<CarrierStatusRule> entity)
    {
        entity.ToTable("carrier_status_rules", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("carrier_status_rule_id");

        entity.AuditProperties();

        entity.Property(e => e.CarrierId)
            .HasColumnName("carrier_id");

        entity.HasOne(e => e.Carrier)
            .WithMany(e => e.Rules)
            .HasForeignKey(e => e.CarrierId);

        entity.Property(e => e.Pattern)
            .HasColumnName("pattern");

        entity.Property(e => e.StatusTypeId)
            .HasColumnName("status_type_id");

        entity.HasOne(e => e.StatusType)
            .WithMany()
            .HasForeignKey(e => e.StatusTypeId);

        entity.Property(e => e.Priority)
            .HasColumnName("priority");

        entity.Property(e => e.IsActive)
            .HasColumnName("is_active");
    }
}
