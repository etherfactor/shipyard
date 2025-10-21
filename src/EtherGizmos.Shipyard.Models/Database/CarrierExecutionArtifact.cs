using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Database;

public class CarrierExecutionArtifact : Auditable, IEntity
{
    public virtual int Id { get; set; }

    public virtual int CarrierExecutionId { get; set; }

    public virtual CarrierExecution CarrierExecution { get; set; } = null!;

    public virtual string ArtifactUri { get; set; } = null!;

    public virtual short StepIndex { get; set; }
}
