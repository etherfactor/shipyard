using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.Shipyard.Models;

public class CarrierExecutionArtifactDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var complex = builder.ComplexType<CarrierExecutionArtifactDTO>();

        complex.Namespace = "EtherGizmos.Shipyard";
        complex.Name = complex.Name.Replace("DTO", "");

        complex.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            /* Begin Audit */
            /*  End Audit  */
            complex.Property(e => e.ArtifactUri);
            complex.Property(e => e.ContentType);
            complex.Property(e => e.FileName);
            complex.Property(e => e.Bytes);
            complex.Property(e => e.StepIndex);
        }
    }
}
