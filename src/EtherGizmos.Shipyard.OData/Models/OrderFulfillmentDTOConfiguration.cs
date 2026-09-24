using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.Shipyard.Models;

public class OrderFulfillmentDTOConfiguration : IModelConfiguration
{
    public void Apply(
        ODataModelBuilder builder,
        ApiVersion apiVersion,
        string? routePrefix)
    {
        var entitySet = builder.EntitySet<OrderFulfillmentDTO>("orderFulfillments");
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
            entity.Property(e => e.Sender);
            entity.EnumProperty(e => e.OrderFulfillmentStatusType);
            entity.Property(e => e.ShippedOn);
            entity.Property(e => e.ReceivedAt);
            entity.Property(e => e.ReceivedByUserId);
            entity.HasOptional(e => e.ReceivedByUser);
            entity.Property(e => e.ResolvedAt);
            entity.Property(e => e.ResolvedByUserId);
            entity.HasOptional(e => e.ResolvedByUser);
            entity.Property(e => e.ResolvedReason);
            entity.CollectionProperty(e => e.OrderFulfillmentItems);
        }
    }
}
