namespace EtherGizmos.Shipyard.Models.Database.Base;

public abstract class Auditable
{
    public virtual DateTimeOffset CreatedAt { get; set; }

    public virtual DateTimeOffset ModifiedAt { get; set; }
}
