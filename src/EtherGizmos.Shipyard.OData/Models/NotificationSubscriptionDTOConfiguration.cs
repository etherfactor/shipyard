using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.Shipyard.Models;

public class NotificationSubscriptionDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var entitySet = builder.EntitySet<NotificationSubscriptionDTO>("notificationSubscriptions");
        var entity = entitySet.EntityType;

        entity.Namespace = "EtherGizmos.Shipyard";
        entity.Name = entity.Name.Replace("DTO", "");

        entity.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id);
            /* Begin Audit */
            /*  End Audit  */
            entity.Property(e => e.UserId);
            entity.Property(e => e.EventType);
            entity.Property(e => e.ChannelKey);
            entity.ComplexProperty(e => e.ChannelConfig);
            entity.Property(e => e.ScheduleType);
            entity.ComplexProperty(e => e.ScheduleConfig);
            entity.Property(e => e.IsActive);
            entity.Property(e => e.LastNotificationAt);
            entity.Property(e => e.NextNotificationAt);
        }
    }
}
