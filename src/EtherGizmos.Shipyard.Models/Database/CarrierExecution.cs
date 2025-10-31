using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Database.Enums;

namespace EtherGizmos.Shipyard.Database;

public class CarrierExecution : Auditable, IEntity
{
    public virtual int Id { get; set; }

    public virtual int CarrierId { get; set; }

    public virtual Carrier Carrier { get; set; } = null!;

    public virtual DateTimeOffset? StartedAt { get; set; }

    public virtual DateTimeOffset? CompletedAt { get; set; }

    public virtual ExecutionStatusType ExecutionStatus { get; set; } = ExecutionStatusType.Queued;

    public virtual short StepCount { get; set; }

    public virtual short? FailureStepIndex { get; set; }

    public virtual List<CarrierExecutionArtifact> Artifacts { get; set; } = [];
}
