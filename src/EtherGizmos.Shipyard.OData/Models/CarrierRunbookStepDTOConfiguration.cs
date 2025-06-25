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
            //Due to a bug, we have to omit this and allow the convention builder to add it. This has been patched for when
            //the next OData model builder package release
            //complex.HasDynamicProperties(e => e.Payload);
        }
    }
}
