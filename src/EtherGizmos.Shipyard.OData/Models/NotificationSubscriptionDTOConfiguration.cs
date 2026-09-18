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
            entity.Property(e => e.NotificationEventId);
            entity.HasRequired(e => e.NotificationEvent);
            entity.ComplexProperty(e => e.NotificationEventConfig);
            entity.Property(e => e.NotificationChannelId);
            entity.HasRequired(e => e.NotificationChannel);
            entity.ComplexProperty(e => e.NotificationChannelConfig);
            entity.Property(e => e.NotificationScheduleId);
            entity.HasRequired(e => e.NotificationSchedule);
            entity.ComplexProperty(e => e.NotificationScheduleConfig);
            entity.Property(e => e.IsActive);
            entity.Property(e => e.LastNotificationAt);
            entity.Property(e => e.NextNotificationAt);
        }
    }
}
