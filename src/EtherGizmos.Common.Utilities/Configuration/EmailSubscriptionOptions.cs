using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Common.Configuration;

public class EmailSubscriptionOptions
{
    [Required]
    public string ConnectionId { get; set; } = null!;

    public List<string> To { get; set; } = [];

    public List<string> Cc { get; set; } = [];

    public List<string> Bcc { get; set; } = [];

    public List<string> NotifyOn { get; set; } = ["OutForDelivery", "Delivered", "FailedAttempt", "Returned"];
}
