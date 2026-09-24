using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.Shipyard.Models;

public class OrderDTOConfiguration : IModelConfiguration
{
    public void Apply(
        ODataModelBuilder builder,
        ApiVersion apiVersion,
        string? routePrefix)
    {
        var entitySet = builder.EntitySet<OrderDTO>("orders");
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
            entity.Property(e => e.Source);
            entity.Property(e => e.OrderNumber);
            entity.Property(e => e.OrderedOn);
            entity.Property(e => e.TotalPrice);
            entity.Property(e => e.OrderUrl);
            entity.Property(e => e.ExpectedShipStartOn);
            entity.Property(e => e.ExpectedShipEndOn);
            entity.CollectionProperty(e => e.OrderItems);
        }
    }
}
