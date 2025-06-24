using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.Shipyard.Models;
using EtherGizmos.Shipyard.Models.Api;
using EtherGizmos.Shipyard.OData.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.Shipyard.OData.Models;

public class CarrierRunbookStepDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var complex = builder.ComplexType<CarrierRunbookStepDTO>();

        complex.Namespace = "EtherGizmos.Shipyard";
        complex.Name = complex.Name.Replace("DTO", "");

        complex.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            /* Begin Audit */
            /*  End Audit  */
            complex.EnumProperty(e => e.StepType);
            complex.Property(e => e.From);
            complex.Property(e => e.Name);
            complex.Property(e => e.Selector);
            complex.CollectionProperty(e => e.Steps);
            complex.Property(e => e.To);
            complex.Property(e => e.Trim);
            complex.Property(e => e.Url);
            complex.Property(e => e.Value);
            complex.Property(e => e.Var);
        }
    }
}
