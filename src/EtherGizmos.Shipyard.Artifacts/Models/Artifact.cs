using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class Artifact : Auditable, IEntity
{
    public Guid Id { get; set; }

    public string Uri { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public long Bytes { get; set; }

    public string PhysicalPath { get; set; } = null!;
}

public class ArtifactConfiguration : IEntityTypeConfiguration<Artifact>
{
    public void Configure(
        EntityTypeBuilder<Artifact> entity)
    {
        entity.ToTable("artifacts", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("artifact_id");

        entity.AuditProperties();

        entity.Property(e => e.Uri)
            .HasColumnName("uri");

        entity.HasAlternateKey(e => e.Uri);

        entity.Property(e => e.ContentType)
            .HasColumnName("content_type");

        entity.Property(e => e.FileName)
            .HasColumnName("file_name");

        entity.Property(e => e.Bytes)
            .HasColumnName("bytes");

        entity.Property(e => e.PhysicalPath)
            .HasColumnName("physical_path");
    }
}
