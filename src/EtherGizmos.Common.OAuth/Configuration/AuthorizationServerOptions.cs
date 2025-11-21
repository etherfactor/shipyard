using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EtherGizmos.Common.Configuration;

public class AuthorizationServerOptions
{
    public CookieOptions Cookie { get; set; } = new();

    public OAuth2Options OAuth2 { get; set; } = new();

    public List<Assembly> ScanAssemblies { get; set; } = [];

    public class CookieOptions
    {
        [Required]
        public string LoginUrl { get; set; } =
            AuthorizationConstants.Cookie.ControllerPath + "/" + AuthorizationConstants.Cookie.LoginPath;

        [Required]
        public string LogoutUrl { get; set; } =
            AuthorizationConstants.Cookie.ControllerPath + "/" + AuthorizationConstants.Cookie.LogoutPath;

        [Required]
        public string ReturnUrlParameter { get; set; } = AuthorizationConstants.Cookie.ReturnUrlParameter;

        [Required]
        public TimeSpan CookieLifetime { get; set; } = TimeSpan.FromHours(1);

        public bool SlidingExpiration { get; set; } = false;
    }

    public class OAuth2Options
    {
        [Required]
        public Type DbContextType { get; set; } = null!;

        [Required]
        public CertificateOptions SigningCertificate { get; set; } = new();

        [Required]
        public CertificateOptions EncryptionCertificate { get; set; } = new();

        [Required]
        public string AuthorizationEndpointUrl { get; set; } =
            AuthorizationConstants.OAuth2.ControllerPath + "/" + AuthorizationConstants.OAuth2.AuthorizePath;

        [Required]
        public string TokenEndpointUrl { get; set; } =
            AuthorizationConstants.OAuth2.ControllerPath + "/" + AuthorizationConstants.OAuth2.TokenPath;

        [Required]
        public string IntrospectionEndpointUrl { get; set; } =
            AuthorizationConstants.OAuth2.ControllerPath + "/" + AuthorizationConstants.OAuth2.IntrospectPath;

        [Required]
        public string RevocationEndpointUrl { get; set; } =
            AuthorizationConstants.OAuth2.ControllerPath + "/" + AuthorizationConstants.OAuth2.RevokePath;

        [Required]
        public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromHours(1);

        [Required]
        public TimeSpan IdentityTokenLifetime { get; set; } = TimeSpan.FromHours(1);

        [Required]
        public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(90);

        public bool DisableTransportSecurityRequirement { get; set; } = false;
    }

    public class CertificateOptions
    {
        [Required]
        public string CertificateId { get; set; } = null!;
    }
}
