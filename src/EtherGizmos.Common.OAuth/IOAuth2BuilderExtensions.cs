using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Configuration;
using EtherGizmos.Common.Extensions;
using EtherGizmos.Common.Models;
using EtherGizmos.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common;

public static class IOAuth2BuilderExtensions
{
    public static IOAuth2Builder AsAuthorizationServer<TContext>(
        this IOAuth2Builder @this,
        Action<AuthorizationServerOptions> configureOptions)
        where TContext : AuthorizationContext
    {
        var tempOptions = new AuthorizationServerOptions();
        configureOptions(tempOptions);
        tempOptions.OAuth2.DbContextType = typeof(TContext);

        @this.Builder.Services.AddOpenIddict()
            .AddCore(opt =>
            {
                opt.UseEntityFrameworkCore()
                    .UseDbContext(tempOptions.OAuth2.DbContextType)
                    .ReplaceDefaultEntities<OAuth2Application, OAuth2Authorization, OAuth2Scope, OAuth2Token, int>();
            })
            .AddServer(opt =>
            {
                opt.AllowAuthorizationCodeFlow()
                    .RequireProofKeyForCodeExchange();

                opt.AllowRefreshTokenFlow();

                opt.SetAuthorizationEndpointUris(tempOptions.OAuth2.AuthorizationEndpointUrl);
                opt.SetTokenEndpointUris(tempOptions.OAuth2.TokenEndpointUrl);
                opt.SetIntrospectionEndpointUris(tempOptions.OAuth2.IntrospectionEndpointUrl);
                opt.SetRevocationEndpointUris(tempOptions.OAuth2.RevocationEndpointUrl);

                opt.SetAccessTokenLifetime(tempOptions.OAuth2.AccessTokenLifetime);
                opt.SetIdentityTokenLifetime(tempOptions.OAuth2.IdentityTokenLifetime);
                opt.SetRefreshTokenLifetime(tempOptions.OAuth2.RefreshTokenLifetime);

                opt.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableTokenEndpointPassthrough();

                if (tempOptions.OAuth2.DisableTransportSecurityRequirement)
                {
                    opt.UseAspNetCore()
                        .DisableTransportSecurityRequirement();
                }
            });

        @this.Builder.Services.AddAuthentication(AuthorizationConstants.OAuth2.AuthenticationScheme)
            .AddCookie(AuthorizationConstants.Cookie.AuthenticationScheme, opt =>
            {
                var login = tempOptions.Cookie.LoginUrl;
                if (login[0] != '/')
                    login = '/' + login;

                var logout = tempOptions.Cookie.LogoutUrl;
                if (logout[0] != '/')
                    logout = '/' + logout;

                opt.ReturnUrlParameter = tempOptions.Cookie.ReturnUrlParameter;
                opt.ExpireTimeSpan = tempOptions.Cookie.CookieLifetime;
                opt.SlidingExpiration = tempOptions.Cookie.SlidingExpiration;
            });

        var isIntegration = false;
        if (isIntegration)
        {
            @this.Builder.Services.AddAuthentication()
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
            @this.Builder.Services.AddOpenIddict()
                .AddValidation(opt =>
                {
                    opt.UseLocalServer();
                    opt.UseAspNetCore();
                });
        }

        @this.Builder.Services.AddAuthorization();

        return @this;
    }
}
