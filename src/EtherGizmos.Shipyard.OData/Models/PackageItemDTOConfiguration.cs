using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.Shipyard.Models;

public class PackageItemDTOConfiguration : IModelConfiguration
{
    public void Apply(
        ODataModelBuilder builder,
        ApiVersion apiVersion,
        string? routePrefix)
    {
        var entitySet = builder.EntitySet<PackageItemDTO>("packageItems");
        var entity = entitySet.EntityType;

        entity.Namespace = "EtherGizmos.Shipyard";
        entity.Name = entity.Name.Replace("DTO", "");

        entity.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            entity.Property(e => e.Id);
            /* Begin Audit */
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.ModifiedAt);
            /*  End Audit  */
            entity.Property(e => e.PackageId);
            entity.HasRequired(e => e.Package);
            entity.Property(e => e.OrderFulfillmentItemId);
            entity.HasOptional(e => e.OrderFulfillmentItem);
            entity.Property(e => e.Description);
            entity.Property(e => e.Quantity);
            entity.Property(e => e.QuantityUnits);
        }
    }
}
