namespace EtherGizmos.Shipyard.Models.Database;

/// <summary>
/// Normally, this would be an enum, but we also want additional metadata, so we have to settle for a class. Doesn't change
/// anything in the database, though.
/// </summary>
public class ShipmentStatusType
{
    public virtual int Id { get; set; }

    public virtual string Name { get; set; } = null!;

    public virtual string? Description { get; set; }

    public virtual decimal PollingInterval { get; set; }
}

//Base polling rate: 6h (I don't want to be spammy), probably configurable

/*
 * Statuses to insert:
 * 0 - Unknown
 *   poll interval: 4
 * 10 - Waiting (not yet shipped)
 *   poll interval: 2
 * 20 - In transit
 *   poll interval: 1
 * 30 - Out for delivery
 *   poll interval: 0.167
 * 100 - Delivered
 *   poll interval: never
 * -10 - Failed attempt
 *   poll interval: 1
 * -100 - Returned
 *   poll interval: never
 * -200 - Expired
 *   poll interval: never
 */
