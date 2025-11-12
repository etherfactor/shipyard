using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.Shipyard.Models;

public class UserDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var entitySet = builder.EntitySet<UserDTO>("users");
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
            entity.Property(e => e.Username);
            entity.Property(e => e.Password);
            entity.Property(e => e.EmailAddress);
            entity.Property(e => e.GivenName);
            entity.Property(e => e.FamilyName);
            entity.Property(e => e.FullName);
        }
    }
}
