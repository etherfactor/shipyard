using Microsoft.AspNetCore.Authentication.Cookies;
using OpenIddict.Validation.AspNetCore;

namespace EtherGizmos.Common;

internal static class AuthorizationConstants
{
    public static class Cookie
    {
        public const string AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        public const string ControllerPath = "account";

        public const string ReturnUrlParameter = "returnUrl";

        public const string LoginPath = "login";

        public const string LogoutPath = "logout";
    }

    public static class OAuth2
    {
        public const string AuthenticationScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;

        public const string ControllerPath = "oauth/v2.0";

        public const string AuthorizePath = "authorize";

        public const string TokenPath = "token";

        public const string IntrospectPath = "introspect";

        public const string RevokePath = "revoke";
    }
}
