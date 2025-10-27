using EtherGizmos.Shipyard.Utilities.Abstractions;

namespace EtherGizmos.Shipyard.Models.Database;

public class Carrier : Auditable, IEntity
{
    public virtual int Id { get; set; }

    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// The intention is some sort of public, global identifier that distinguishes carriers. That way, we don't need to use
    /// a string id, but we can still uniquely identify them via an alternate key as needed.
    /// </summary>
    public virtual string Slug { get; set; } = null!;

    public virtual List<CarrierRunbookStep> Steps { get; set; } = [];

    public virtual List<CarrierStatusRule> Rules { get; set; } = [];
}
