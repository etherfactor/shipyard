using Microsoft.AspNetCore.Authentication.Cookies;
using OpenIddict.Validation.AspNetCore;

namespace EtherGizmos.Common;

internal static class AuthorizationConstants
{
    public static class Cookie
    {
        public const string AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }

    public static class OAuth2
    {
        public const string AuthenticationScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;

        public const string ControllerPath = "oauth/v2.0";

        public const string AuthorizePath = ControllerPath + "/authorize";

        public const string TokenPath = ControllerPath + "/token";
    }
}
