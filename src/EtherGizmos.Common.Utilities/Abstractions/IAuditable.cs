using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.Common.Abstractions;

public interface IAuditable
{
    [Column("created_at_utc")]
    DateTimeOffset? CreatedAt { get; set; }

    [Column("created_by_user_id")]
    Guid? CreatedByUserId { get; set; }

    [Column("modified_at_utc")]
    DateTimeOffset? ModifiedAt { get; set; }

    [Column("modified_by_user_id")]
    Guid? ModifiedByUserId { get; set; }
}
