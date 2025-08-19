using EtherGizmos.Common.Models.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenIddict.EntityFrameworkCore.Models;

namespace EtherGizmos.Common.Models;

public class OAuth2Authorization : OpenIddictEntityFrameworkCoreAuthorization<int, OAuth2Application, OAuth2Token>
{
}

public class OAuth2AuthorizationConfiguration : IEntityTypeConfiguration<OAuth2Authorization>
{
    public void Configure(EntityTypeBuilder<OAuth2Authorization> entity)
    {
        entity.ToTable("authorizations", schema: "oauth2");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("authorization_id");

        entity.Property(e => e.CreationDate)
            .HasColumnName("created_at_utc")
            .HasDefaultValueSql();

        entity.HasOne(e => e.Application)
            .WithMany(e => e.Authorizations)
            .HasForeignKey("application_id");

        entity.Property(e => e.ConcurrencyToken)
            .HasColumnName("concurrency_token");

        entity.Property(e => e.Properties)
            .HasColumnName("properties");

        entity.Property(e => e.Scopes)
            .HasColumnName("scopes");

        entity.Property(e => e.Status)
            .HasColumnName("status_type_id")
            .HasConversion(new OAuth2StatusTypeValueConverter());

        entity.Property(e => e.Subject)
            .HasColumnName("subject");

        entity.Property(e => e.Type)
            .HasColumnName("authorization_type_id")
            .HasConversion(new OAuth2AuthorizationTypeValueConverter());
    }
}
