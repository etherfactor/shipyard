using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.Shipyard.Models;
using EtherGizmos.Shipyard.Models.Api;
using EtherGizmos.Shipyard.OData.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.Shipyard.OData.Models;

public class CarrierStatusRuleDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var complex = builder.ComplexType<CarrierStatusRuleDTO>();

        complex.Namespace = "EtherGizmos.Shipyard";
        complex.Name = complex.Name.Replace("DTO", "");

        complex.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            /* Begin Audit */
            complex.Property(e => e.CreatedAt);
            complex.Property(e => e.ModifiedAt);
            /*  End Audit  */
            complex.Property(e => e.Pattern);
            complex.EnumProperty(e => e.StatusType);
            complex.Property(e => e.Priority);
            complex.Property(e => e.IsActive);
        }
    }
}
