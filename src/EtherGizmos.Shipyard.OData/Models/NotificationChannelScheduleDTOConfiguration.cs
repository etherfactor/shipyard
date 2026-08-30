using Asp.Versioning;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.Shipyard.Models;

public class NotificationChannelScheduleDTOConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var complex = builder.ComplexType<NotificationChannelScheduleDTO>();

        complex.Namespace = "EtherGizmos.Shipyard";
        complex.Name = complex.Name.Replace("DTO", "");

        complex.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            complex.Property(e => e.NotificationChannelId);
            complex.HasRequired(e => e.NotificationChannel);
            complex.Property(e => e.NotificationScheduleId);
            complex.HasRequired(e => e.NotificationSchedule);
        }
    }
}
