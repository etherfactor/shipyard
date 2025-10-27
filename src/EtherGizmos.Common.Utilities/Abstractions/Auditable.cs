namespace EtherGizmos.Common.Abstractions;

public abstract class Auditable : IAuditable
{
    public virtual DateTimeOffset? CreatedAt { get; set; }

    public virtual Guid? CreatedByUserId { get; set; }

    public virtual DateTimeOffset? ModifiedAt { get; set; }

    public virtual Guid? ModifiedByUserId { get; set; }
}
