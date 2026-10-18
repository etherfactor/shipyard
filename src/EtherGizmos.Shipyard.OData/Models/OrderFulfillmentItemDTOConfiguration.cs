using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.Shipyard.Models;

public class OrderFulfillmentItemDTOConfiguration : IModelConfiguration
{
    public void Apply(
        ODataModelBuilder builder,
        ApiVersion apiVersion,
        string? routePrefix)
    {
        var entitySet = builder.EntitySet<OrderFulfillmentItemDTO>("orderFulfillmentItems");
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
            entity.Property(e => e.OrderFulfillmentId);
            entity.HasRequired(e => e.OrderFulfillment);
            entity.Property(e => e.OrderItemId);
            entity.HasRequired(e => e.OrderItem);
            entity.Property(e => e.ExpectedQuantity);
            entity.Property(e => e.ReceivedQuantity);
        }
    }
}
