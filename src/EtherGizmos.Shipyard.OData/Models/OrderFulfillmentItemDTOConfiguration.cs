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
        var complex = builder.ComplexType<OrderFulfillmentItemDTO>();

        complex.Namespace = "EtherGizmos.Shipyard";
        complex.Name = complex.Name.Replace("DTO", "");

        complex.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            complex.Property(e => e.OrderItemId);
            complex.HasRequired(e => e.OrderItem);
            complex.Property(e => e.ExpectedQuantity);
            complex.Property(e => e.ReceivedQuantity);
        }
    }
}
