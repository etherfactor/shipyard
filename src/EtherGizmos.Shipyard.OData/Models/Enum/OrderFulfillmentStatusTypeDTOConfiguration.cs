using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.Shipyard.Api.Enums;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.Shipyard.Models.Enum;

public class OrderFulfillmentStatusTypeDTOConfiguration : IModelConfiguration
{
    public void Apply(
        ODataModelBuilder builder,
        ApiVersion apiVersion,
        string? routePrefix)
    {
        var enumType = builder.EnumType<OrderFulfillmentStatusTypeDTO>();

        enumType.Namespace = "EtherGizmos.Shipyard";
        enumType.Name = enumType.Name.Replace("DTO", "");
    }
}
