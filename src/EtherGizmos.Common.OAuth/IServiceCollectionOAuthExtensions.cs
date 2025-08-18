using EtherGizmos.Common.Extensions;
using EtherGizmos.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common;

public static class IServiceCollectionOAuthExtensions
{
    public static IHostApplicationBuilder UseOAuth2(
        this IHostApplicationBuilder @this)
    {
        @this.Services.AddOpenIddict()
            .AddCore(opt =>
            {
                var options = new
                {
                    OAuth2 = new
                    {
                        DbContextType = typeof(IServiceCollection),
                    },
                };
                opt.UseEntityFrameworkCore()
                    .UseDbContext(options.OAuth2.DbContextType)
                    .ReplaceDefaultEntities<OAuth2Application, OAuth2Authorization, OAuth2Scope, OAuth2Token, int>();
            })
            .AddServer(opt =>
            {
                var options = new
                {
                    OAuth2 = new
                    {
                        AuthorizationEndpointUrl = "",
                        TokenEndpointUrl = "",
                        IntrospectionEndpointUrl = "",
                        RevocationEndpointUrl = "",
                        AccessTokenLifetime = TimeSpan.Zero,
                        IdentityTokenLifetime = TimeSpan.Zero,
                        RefreshTokenLifetime = TimeSpan.Zero,
                        DisableTransportSecurityRequirement = false,
                    },
                };
                opt.AllowAuthorizationCodeFlow()
                    .RequireProofKeyForCodeExchange();

                opt.AllowRefreshTokenFlow();

                opt.SetAuthorizationEndpointUris(options.OAuth2.AuthorizationEndpointUrl);
                opt.SetTokenEndpointUris(options.OAuth2.TokenEndpointUrl);
                opt.SetIntrospectionEndpointUris(options.OAuth2.IntrospectionEndpointUrl);
                opt.SetRevocationEndpointUris(options.OAuth2.RevocationEndpointUrl);

                opt.SetAccessTokenLifetime(options.OAuth2.AccessTokenLifetime);
                opt.SetIdentityTokenLifetime(options.OAuth2.IdentityTokenLifetime);
                opt.SetRefreshTokenLifetime(options.OAuth2.RefreshTokenLifetime);

                opt.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableTokenEndpointPassthrough();

                if (options.OAuth2.DisableTransportSecurityRequirement)
                {
                    opt.UseAspNetCore()
                        .DisableTransportSecurityRequirement();
                }
            });

        @this.Services.AddAuthentication(AuthorizationConstants.OAuth2.AuthenticationScheme)
            .AddCookie(AuthorizationConstants.Cookie.AuthenticationScheme, opt =>
            {
                var options = new
                {
                    Cookie = new
                    {
                        LoginPath = "/",
                        LogoutPath = "/",
                        ReturnUrlParameter = "returnUrl",
                        CookieLifetime = TimeSpan.Zero,
                        SlidingExpiration = true,
                    },
                };
                var login = options.Cookie.LoginPath;
                if (login[0] != '/')
                    login = '/' + login;

                var logout = options.Cookie.LogoutPath;
                if (logout[0] != '/')
                    logout = '/' + logout;

                opt.ReturnUrlParameter = options.Cookie.ReturnUrlParameter;
                opt.ExpireTimeSpan = options.Cookie.CookieLifetime;
                opt.SlidingExpiration = options.Cookie.SlidingExpiration;
            });

        var isIntegration = false;
        if (isIntegration)
        {
            @this.Services.AddAuthentication()
                .AddJwtBearer(opt =>
                {
                    opt.MapInboundClaims = false;

                    opt.TokenValidationParameters = new()
                    {
                        TokenDecryptionKey = null,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = null,

                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,

                        ClockSkew = TimeSpan.Zero,

                        NameClaimType = Claims.Subject,
                        RoleClaimType = Claims.Role,
                    };
                });
        }
        else
        {
            @this.Services.AddOpenIddict()
                .AddValidation(opt =>
                {
                    opt.UseLocalServer();
                    opt.UseAspNetCore();
                });
        }

        @this.Services.AddAuthorization();

        return @this;
    }
}
