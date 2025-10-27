using EtherGizmos.Shipyard.Migrations.Core;

namespace EtherGizmos.Shipyard.Database.Migrations._00._00.Feature000000;

[CreatedAt(year: 2025, month: 08, day: 18, hour: 18, minute: 30, description: "Load OAuth 2.0 tables")]
public class Migration004_LoadOAuthTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Load [oauth2].[application_types]  (enum OAuth2ApplicationType)
         */
        Merge.IntoTable("application_types").InSchema("oauth2")
            .Row(new
            {
                application_type_id = 10,
                name = "Native",
                description = "Installed app (desktop/mobile). Public client; cannot keep a client secret; MUST use PKCE."
            })
            .Row(new
            {
                application_type_id = 20,
                name = "Web",
                description = "Server-side web app. Confidential client; can keep a client secret; typically uses auth code + PKCE."
            })
            .Match(e => new { e.application_type_id });

        /*
         * Load [oauth2].[authorization_types]  (enum OAuth2AuthorizationType)
         */
        Merge.IntoTable("authorization_types").InSchema("oauth2")
            .Row(new
            {
                authorization_type_id = 10,
                name = "Permanent",
                description = "Long-lived grant for a subject+client; can be reused for silent sign-in and incremental consent."
            })
            .Row(new
            {
                authorization_type_id = 20,
                name = "AdHoc",
                description = "Ephemeral grant used for immediate token issuance only; not reused for later requests."
            })
            .Match(e => new { e.authorization_type_id });

        /*
         * Load [oauth2].[client_types]  (enum OAuth2ClientType)
         */
        Merge.IntoTable("client_types").InSchema("oauth2")
            .Row(new
            {
                client_type_id = 10,
                name = "Public",
                description = "Cannot keep a secret (e.g., SPA, native). Uses PKCE; no client_secret in production."
            })
            .Row(new
            {
                client_type_id = 20,
                name = "Confidential",
                description = "Can keep a secret (e.g., backend). Authenticates with client_secret/private key."
            })
            .Match(e => new { e.client_type_id });

        /*
         * Load [oauth2].[consent_types]  (enum OAuth2ConsentType)
         */
        Merge.IntoTable("consent_types").InSchema("oauth2")
            .Row(new
            {
                consent_type_id = 10,
                name = "Implicit",
                description = "No interactive consent; server auto-grants according to policy."
            })
            .Row(new
            {
                consent_type_id = 20,
                name = "Explicit",
                description = "End-user must grant consent; subsequent requests may be silent if scopes unchanged."
            })
            .Row(new
            {
                consent_type_id = 30,
                name = "External",
                description = "Consent is managed by an external system/IdP; this server does not prompt."
            })
            .Row(new
            {
                consent_type_id = 40,
                name = "Systematic",
                description = "Always prompt for consent on each authorization request."
            })
            .Match(e => new { e.consent_type_id });

        /*
         * Load [oauth2].[status_types]  (enum OAuth2StatusType)
         */
        Merge.IntoTable("status_types").InSchema("oauth2")
            .Row(new { status_type_id = 10, name = "Valid", description = "Active/usable." })
            .Row(new { status_type_id = 20, name = "Inactive", description = "Disabled/not usable yet." })
            .Row(new { status_type_id = 30, name = "Redeemed", description = "Used (e.g., authorization code or refresh token already exchanged)." })
            .Row(new { status_type_id = 40, name = "Rejected", description = "Denied by the authorization server." })
            .Row(new { status_type_id = 50, name = "Revoked", description = "Explicitly invalidated after issuance." })
            .Match(e => new { e.status_type_id });

        /*
         * Load [oauth2].[token_types]  (enum OAuth2TokenType)
         */
        Merge.IntoTable("token_types").InSchema("oauth2")
            .Row(new
            {
                token_type_id = 10,
                name = "Bearer",
                description = "Bearer token scheme (RFC 6750) used for access tokens."
            })
            .Row(new
            {
                token_type_id = 20,
                name = "AccessToken",
                description = "Token granting API access; self-contained (JWT) or reference."
            })
            .Row(new
            {
                token_type_id = 21,
                name = "IdentityToken",
                description = "OIDC ID Token describing the authenticated user (subject/claims)."
            })
            .Row(new
            {
                token_type_id = 22,
                name = "RefreshToken",
                description = "Token used to obtain new access/ID tokens after expiry."
            })
            .Row(new
            {
                token_type_id = 30,
                name = "StateToken",
                description = "Opaque state value for request correlation/CSRF protection; not redeemable."
            })
            .Row(new
            {
                token_type_id = 40,
                name = "AuthorizationCode",
                description = "Short-lived code exchanged for tokens in the authorization code flow."
            })
            .Match(e => new { e.token_type_id });
    }

    public override void Down()
    {
        //No-op
    }
}
