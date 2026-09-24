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
        var complex = builder.ComplexType<PackageItemDTO>();

        complex.Namespace = "EtherGizmos.Shipyard";
        complex.Name = complex.Name.Replace("DTO", "");

        complex.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            complex.Property(e => e.OrderFulfillmentItemId);
            complex.HasOptional(e => e.OrderFulfillmentItem);
            complex.Property(e => e.Description);
            complex.Property(e => e.Quantity);
            complex.Property(e => e.QuantityUnits);
        }
    }
}
