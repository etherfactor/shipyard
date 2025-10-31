using EtherGizmos.Common.Models.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenIddict.EntityFrameworkCore.Models;

namespace EtherGizmos.Common.Models;

public class OAuth2Token : OpenIddictEntityFrameworkCoreToken<int, OAuth2Application, OAuth2Authorization>
{
}

public class OAuth2TokenConfiguration : IEntityTypeConfiguration<OAuth2Token>
{
    public void Configure(EntityTypeBuilder<OAuth2Token> entity)
    {
        entity.ToTable("tokens", schema: "oauth2");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("token_id");

        entity.Property(e => e.CreationDate)
            .HasColumnName("created_at_utc")
            .HasDefaultValueSql();

        entity.HasOne(e => e.Application)
            .WithMany(e => e.Tokens)
            .HasForeignKey("application_id");

        entity.HasOne(e => e.Authorization)
            .WithMany(e => e.Tokens)
            .HasForeignKey("authorization_id");

        entity.Property(e => e.ConcurrencyToken)
            .HasColumnName("concurrency_token");

        entity.Property(e => e.ExpirationDate)
            .HasColumnName("expires_at_utc");

        entity.Property(e => e.Payload)
            .HasColumnName("payload");

        entity.Property(e => e.Properties)
            .HasColumnName("properties");

        entity.Property(e => e.RedemptionDate)
            .HasColumnName("redeemed_at_utc");

        entity.Property(e => e.ReferenceId)
            .HasColumnName("reference_id");

        entity.Property(e => e.Status)
            .HasColumnName("status_type_id")
            .HasConversion(new OAuth2StatusTypeValueConverter());

        entity.Property(e => e.Subject)
            .HasColumnName("subject");

        entity.Property(e => e.Type)
            .HasColumnName("token_type_id")
            .HasConversion(new OAuth2TokenTypeValueConverter());
    }
}
