using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class CarrierExecutionConfiguration : IEntityTypeConfiguration<CarrierExecution>
{
    public void Configure(
        EntityTypeBuilder<CarrierExecution> entity)
    {
        entity.ToTable("carrier_executions", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("carrier_execution_id");

        entity.AuditProperties();

        entity.Property(e => e.CarrierId)
            .HasColumnName("carrier_id");

        entity.HasOne(e => e.Carrier)
            .WithMany()
            .HasForeignKey(e => e.CarrierId);

        entity.Property(e => e.StartedAt)
            .HasColumnName("started_at_utc");

        entity.Property(e => e.CompletedAt)
            .HasColumnName("completed_at_utc");

        entity.Property(e => e.ExecutionStatus)
            .HasColumnName("execution_status_type_id");

        entity.Property(e => e.StepCount)
            .HasColumnName("step_count");

        entity.Property(e => e.FailureStepIndex)
            .HasColumnName("failure_step_index");
    }
}
