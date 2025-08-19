using EtherGizmos.Shipyard.Migrations.Core;
using FluentMigrator;

namespace EtherGizmos.Shipyard.Database.Migrations._00._00.Feature000000;

[CreatedAt(year: 2025, month: 08, day: 18, hour: 18, minute: 00, description: "Create OAuth 2.0 tables")]
public class Migration003_CreateOAuthTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [oauth2]
         */
        Create.Schema("oauth2");

        /*
         * Create [oauth2].[application_types]
         */
        Create.Table("application_types").InSchema("oauth2")
            .WithColumn("application_type_id").AsInt32().PrimaryKey()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [oauth2].[authorization_types]
         */
        Create.Table("authorization_types").InSchema("oauth2")
            .WithColumn("authorization_type_id").AsInt32().PrimaryKey()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [oauth2].[client_types]
         */
        Create.Table("client_types").InSchema("oauth2")
            .WithColumn("client_type_id").AsInt32().PrimaryKey()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [oauth2].[consent_types]
         */
        Create.Table("consent_types").InSchema("oauth2")
            .WithColumn("consent_type_id").AsInt32().PrimaryKey()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [oauth2].[status_types]
         */
        Create.Table("status_types").InSchema("oauth2")
            .WithColumn("status_type_id").AsInt32().PrimaryKey()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [oauth2].[token_types]
         */
        Create.Table("token_types").InSchema("oauth2")
            .WithColumn("token_type_id").AsInt32().PrimaryKey()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [oauth2].[applications]
         */
        Create.Table("applications").InSchema("oauth2")
            .WithColumn("application_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("application_type_id").AsInt32().Nullable()
            .WithColumn("client_id").AsGuid().Nullable()
            .WithColumn("client_secret").AsString(int.MaxValue).Nullable()
            .WithColumn("client_type_id").AsInt32().Nullable()
            .WithColumn("concurrency_token").AsString(int.MaxValue).Nullable()
            .WithColumn("consent_type_id").AsInt32().Nullable()
            .WithColumn("display_name").AsString(int.MaxValue).Nullable()
            .WithColumn("display_names").AsString(int.MaxValue).Nullable()
            .WithColumn("json_web_key_set").AsString(int.MaxValue).Nullable()
            .WithColumn("permissions").AsString(int.MaxValue).Nullable()
            .WithColumn("post_logout_redirect_uris").AsString(int.MaxValue).Nullable()
            .WithColumn("properties").AsString(int.MaxValue).Nullable()
            .WithColumn("redirect_uris").AsString(int.MaxValue).Nullable()
            .WithColumn("requirements").AsString(int.MaxValue).Nullable()
            .WithColumn("settings").AsString(int.MaxValue).Nullable();

        Create.Index("UX_applications_client_id")
            .OnTable("applications").InSchema("oauth2")
            .OnColumn("client_id")
            .Unique();

        Create.ForeignKey("FK_applications_application_type_id")
            .FromTable("applications").InSchema("oauth2").ForeignColumn("application_type_id")
            .ToTable("application_types").InSchema("oauth2").PrimaryColumn("application_type_id");

        Create.ForeignKey("FK_applications_client_type_id")
            .FromTable("applications").InSchema("oauth2").ForeignColumn("client_type_id")
            .ToTable("client_types").InSchema("oauth2").PrimaryColumn("client_type_id");

        Create.ForeignKey("FK_applications_consent_type_id")
            .FromTable("applications").InSchema("oauth2").ForeignColumn("consent_type_id")
            .ToTable("consent_types").InSchema("oauth2").PrimaryColumn("consent_type_id");

        /*
         * Create [oauth2].[authorizations]
         */
        Create.Table("authorizations").InSchema("oauth2")
            .WithColumn("authorization_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("created_at_utc").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("application_id").AsInt32().Nullable()
            .WithColumn("concurrency_token").AsString(int.MaxValue).Nullable()
            .WithColumn("properties").AsString(int.MaxValue).Nullable()
            .WithColumn("scopes").AsString(int.MaxValue).Nullable()
            .WithColumn("status_type_id").AsInt32().Nullable()
            .WithColumn("subject").AsString(int.MaxValue).Nullable()
            .WithColumn("authorization_type_id").AsInt32().Nullable();

        Create.Index("IX_authorizations_application_id")
            .OnTable("authorizations").InSchema("oauth2")
            .OnColumn("application_id")
            .Ascending();

        Create.ForeignKey("FK_authorizations_application_id")
            .FromTable("authorizations").InSchema("oauth2").ForeignColumn("application_id")
            .ToTable("applications").InSchema("oauth2").PrimaryColumn("application_id");

        Create.ForeignKey("FK_authorizations_status_type_id")
            .FromTable("authorizations").InSchema("oauth2").ForeignColumn("status_type_id")
            .ToTable("status_types").InSchema("oauth2").PrimaryColumn("status_type_id");

        Create.ForeignKey("FK_authorizations_authorization_type_id")
            .FromTable("authorizations").InSchema("oauth2").ForeignColumn("authorization_type_id")
            .ToTable("authorization_types").InSchema("oauth2").PrimaryColumn("authorization_type_id");

        /*
         * Create [oauth2].[scopes]
         */
        Create.Table("scopes").InSchema("oauth2")
            .WithColumn("scope_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("concurrency_token").AsString(int.MaxValue).Nullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("descriptions").AsString(int.MaxValue).Nullable()
            .WithColumn("display_name").AsString(int.MaxValue).Nullable()
            .WithColumn("display_names").AsString(int.MaxValue).Nullable()
            .WithColumn("name").AsString(int.MaxValue).Nullable()
            .WithColumn("properties").AsString(int.MaxValue).Nullable()
            .WithColumn("resources").AsString(int.MaxValue).Nullable();

        /*
         * Create [oauth2].[tokens]
         */
        Create.Table("tokens").InSchema("oauth2")
            .WithColumn("token_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("created_at_utc").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("application_id").AsInt32().Nullable()
            .WithColumn("authorization_id").AsInt32().Nullable()
            .WithColumn("concurrency_token").AsString(int.MaxValue).Nullable()
            .WithColumn("expires_at_utc").AsDateTime2().Nullable()
            .WithColumn("payload").AsString(int.MaxValue).Nullable()
            .WithColumn("properties").AsString(int.MaxValue).Nullable()
            .WithColumn("redeemed_at_utc").AsDateTime2().Nullable()
            .WithColumn("reference_id").AsString(int.MaxValue).Nullable()
            .WithColumn("status_type_id").AsInt32().Nullable()
            .WithColumn("subject").AsString(int.MaxValue).Nullable()
            .WithColumn("token_type_id").AsInt32().Nullable();

        Create.ForeignKey("FK_tokens_application_id")
            .FromTable("tokens").InSchema("oauth2").ForeignColumn("application_id")
            .ToTable("applications").InSchema("oauth2").PrimaryColumn("application_id");

        Create.ForeignKey("FK_tokens_authorization_id")
            .FromTable("tokens").InSchema("oauth2").ForeignColumn("authorization_id")
            .ToTable("authorizations").InSchema("oauth2").PrimaryColumn("authorization_id");

        Create.ForeignKey("FK_tokens_status_type_id")
            .FromTable("tokens").InSchema("oauth2").ForeignColumn("status_type_id")
            .ToTable("status_types").InSchema("oauth2").PrimaryColumn("status_type_id");

        Create.Index("IX_tokens_subject")
            .OnTable("tokens").InSchema("oauth2")
            .OnColumn("subject")
            .Ascending();

        Create.ForeignKey("FK_tokens_token_type_id")
            .FromTable("tokens").InSchema("oauth2").ForeignColumn("token_type_id")
            .ToTable("token_types").InSchema("oauth2").PrimaryColumn("token_type_id");
    }
}
