using EtherGizmos.Common.Models.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenIddict.EntityFrameworkCore.Models;

namespace EtherGizmos.Common.Models;

public class OAuth2Application : OpenIddictEntityFrameworkCoreApplication<int, OAuth2Authorization, OAuth2Token>
{
}

public class OAuth2ApplicationConfiguration : IEntityTypeConfiguration<OAuth2Application>
{
    public void Configure(EntityTypeBuilder<OAuth2Application> entity)
    {
        entity.ToTable("applications", schema: "oauth2");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("application_id");

        entity.Property(e => e.ApplicationType)
            .HasColumnName("application_type_id")
            .HasConversion(new OAuth2ApplicationTypeValueConverter());

        entity.Property(e => e.ClientId)
            .HasColumnName("client_id")
            .HasConversion(new NullableGuidToStringValueConverter());

        entity.Property(e => e.ClientSecret)
            .HasColumnName("client_secret");

        entity.Property(e => e.ClientType)
            .HasColumnName("client_type_id")
            .HasConversion(new OAuth2ClientTypeValueConverter());

        entity.Property(e => e.ConcurrencyToken)
            .HasColumnName("concurrency_token");

        entity.Property(e => e.ConsentType)
            .HasColumnName("consent_type_id")
            .HasConversion(new OAuth2ConsentTypeValueConverter());

        entity.Property(e => e.DisplayName)
            .HasColumnName("display_name");

        entity.Property(e => e.DisplayNames)
            .HasColumnName("display_names");

        entity.Property(e => e.JsonWebKeySet)
            .HasColumnName("json_web_key_set");

        entity.Property(e => e.Permissions)
            .HasColumnName("permissions");

        entity.Property(e => e.PostLogoutRedirectUris)
            .HasColumnName("post_logout_redirect_uris");

        entity.Property(e => e.Properties)
            .HasColumnName("properties");

        entity.Property(e => e.RedirectUris)
            .HasColumnName("redirect_uris");

        entity.Property(e => e.Requirements)
            .HasColumnName("requirements");

        entity.Property(e => e.Settings)
            .HasColumnName("settings");
    }
}
