using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.Shipyard.Models;

public class CarrierExecutionDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var entitySet = builder.EntitySet<CarrierExecutionDTO>("carrierExecutions");
        var entity = entitySet.EntityType;

        entity.Namespace = "EtherGizmos.Shipyard";
        entity.Name = entity.Name.Replace("DTO", "");

        var readArtifact = entity.Function("readArtifact");
        readArtifact.Parameter<string>("uri");
        readArtifact.Returns<Stream>();

        entity.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id);
            /* Begin Audit */
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.ModifiedAt);
            /*  End Audit  */
            entity.Property(e => e.CarrierId);
            entity.HasRequired(e => e.Carrier);
            entity.Property(e => e.PackageId);
            entity.HasOptional(e => e.Package);
            entity.Property(e => e.StartedAt);
            entity.Property(e => e.CompletedAt);
            entity.EnumProperty(e => e.ExecutionStatusType);
            entity.Property(e => e.StepCount);
            entity.Property(e => e.FailureStepIndex);
            entity.CollectionProperty(e => e.Artifacts);
        }
    }
}
