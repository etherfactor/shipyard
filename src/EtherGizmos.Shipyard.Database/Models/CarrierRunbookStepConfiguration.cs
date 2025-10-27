using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class CarrierRunbookStepConfiguration : IEntityTypeConfiguration<CarrierRunbookStep>
{
    public void Configure(EntityTypeBuilder<CarrierRunbookStep> entity)
    {
        entity.ToTable("carrier_runbook_steps", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("carrier_runbook_step_id");

        entity.Property(e => e.CarrierId)
            .HasColumnName("carrier_id");

        entity.HasOne(e => e.Carrier)
            .WithMany(e => e.Steps)
            .HasForeignKey(e => e.CarrierId);

        entity.Property(e => e.StepType)
            .HasColumnName("step_type_id");

        entity.Property(e => e.Payload)
            .HasColumnName("payload");
    }
}
