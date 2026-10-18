using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.Shipyard.Models;

public class OrderItemDTOConfiguration : IModelConfiguration
{
    public void Apply(
        ODataModelBuilder builder,
        ApiVersion apiVersion,
        string? routePrefix)
    {
        var entitySet = builder.EntitySet<OrderItemDTO>("orderItems");
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
            entity.Property(e => e.OrderId);
            entity.HasRequired(e => e.Order);
            entity.Property(e => e.Description);
            entity.Property(e => e.Quantity);
            entity.Property(e => e.QuantityUnits);
            entity.Property(e => e.UnitPrice);
            entity.Property(e => e.TotalPrice);
        }
    }
}
