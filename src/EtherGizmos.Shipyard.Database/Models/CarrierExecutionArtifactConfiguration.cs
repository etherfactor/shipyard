using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class CarrierExecutionArtifactConfiguration : IEntityTypeConfiguration<CarrierExecutionArtifact>
{
    public void Configure(
        EntityTypeBuilder<CarrierExecutionArtifact> entity)
    {
        entity.ToTable("carrier_execution_artifacts", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("carrier_execution_artifact_id");

        entity.AuditProperties();

        entity.Property(e => e.CarrierExecutionId)
            .HasColumnName("carrier_execution_id");

        entity.HasOne(e => e.CarrierExecution)
            .WithMany(e => e.Artifacts)
            .HasForeignKey(e => e.CarrierExecutionId);

        entity.Property(e => e.ArtifactUri)
            .HasColumnName("artifact_uri");

        entity.Property(e => e.StepIndex)
            .HasColumnName("step_index");
    }
}
