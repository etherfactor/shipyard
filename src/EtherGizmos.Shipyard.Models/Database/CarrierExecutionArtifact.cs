using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Abstractions;

namespace EtherGizmos.Shipyard.Database;

public class CarrierExecutionArtifact : Auditable, IEntity
{
    public virtual int Id { get; set; }

    public virtual int CarrierExecutionId { get; set; }

    public virtual CarrierExecution CarrierExecution { get; set; } = null!;

    public virtual ArtifactUri ArtifactUri { get; set; }

    public virtual string ContentType { get; set; } = null!;

    public virtual long Bytes { get; set; }

    public virtual short StepIndex { get; set; }
}
