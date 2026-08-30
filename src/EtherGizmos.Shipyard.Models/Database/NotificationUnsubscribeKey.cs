using EtherGizmos.Common.Abstractions;
using System.Text;

namespace EtherGizmos.Shipyard.Database;

public class NotificationUnsubscribeKey : IEntity
{
    public NotificationUnsubscribeKey()
    {
        var random = new Random();
        var bytes = new byte[32];

        random.NextBytes(bytes);
        Value = Encoding.UTF8.GetString(bytes);
    }

    public virtual long Id { get; set; }

    public virtual long SubscriptionId { get; set; }

    public virtual string Value { get; set; }
}
