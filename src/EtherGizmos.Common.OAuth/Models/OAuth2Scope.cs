using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenIddict.EntityFrameworkCore.Models;

namespace EtherGizmos.Common.Models;

public class OAuth2Scope : OpenIddictEntityFrameworkCoreScope<int>
{
}

public class OAuth2ScopeConfiguration : IEntityTypeConfiguration<OAuth2Scope>
{
    public void Configure(EntityTypeBuilder<OAuth2Scope> entity)
    {
        entity.ToTable("scopes", schema: "oauth2");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("scope_id");

        entity.Property(e => e.ConcurrencyToken)
            .HasColumnName("concurrency_token");

        entity.Property(e => e.Description)
            .HasColumnName("description");

        entity.Property(e => e.Descriptions)
            .HasColumnName("descriptions");

        entity.Property(e => e.DisplayName)
            .HasColumnName("display_name");

        entity.Property(e => e.DisplayNames)
            .HasColumnName("display_names");

        entity.Property(e => e.Name)
            .HasColumnName("name");

        entity.Property(e => e.Properties)
            .HasColumnName("properties");

        entity.Property(e => e.Resources)
            .HasColumnName("resources");
    }
}
