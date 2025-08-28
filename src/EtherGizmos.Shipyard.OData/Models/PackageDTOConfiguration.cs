using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.Shipyard.Models;

public class PackageDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var entitySet = builder.EntitySet<PackageDTO>("packages");
        var entity = entitySet.EntityType;

        entity.Namespace = "EtherGizmos.Shipyard";
        entity.Name = entity.Name.Replace("DTO", "");

        entity.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id);
            /* Begin Audit */
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.ModifiedAt);
            /*  End Audit  */
            entity.Property(e => e.CarrierId);
            entity.HasRequired(e => e.Carrier);
            entity.Property(e => e.TrackingNumber);
            entity.Property(e => e.Contents);
            entity.Property(e => e.EstimatedDeliveryAt);
            entity.Property(e => e.LastPollAt);
            entity.Property(e => e.NextPollAt);
            entity.EnumProperty(e => e.LastStatusType);
            entity.Property(e => e.IsDelivered);
            entity.HasMany(e => e.TrackingUpdates);

            builder.Function("findUpdatedPackages")
                .ReturnsFromEntitySet<PackageDTO>("packages");

            builder.Action("schedulePoll");
        }
    }
}
