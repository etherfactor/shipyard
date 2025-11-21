using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Configuration;
using EtherGizmos.Common.Extensions;
using EtherGizmos.Common.Models;
using EtherGizmos.Common.Services;
using EtherGizmos.Common.Services.Pipeline.Cookie;
using EtherGizmos.Common.Services.Pipeline.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
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

        var tempServices = new ServiceCollection()
            .AddSingleton<IConfiguration>(@this.Builder.Configuration)
            .AddCertificates();

        using var provider = tempServices.BuildServiceProvider();
        var resolver = provider.GetRequiredService<ICertificateResolver>();

        var encryptionId = tempOptions.OAuth2.EncryptionCertificate.CertificateId;
        var encryption = resolver.LoadCertificate(encryptionId);

        var signingId = tempOptions.OAuth2.SigningCertificate.CertificateId;
        var signing = resolver.LoadCertificate(signingId);

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

                opt.AddEncryptionCertificate(encryption);
                opt.AddSigningCertificate(signing);

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

                opt.LoginPath = login;

                var logout = tempOptions.Cookie.LogoutUrl;
                if (logout[0] != '/')
                    logout = '/' + logout;

                opt.LogoutPath = logout;

                opt.ReturnUrlParameter = tempOptions.Cookie.ReturnUrlParameter;
                opt.ExpireTimeSpan = tempOptions.Cookie.CookieLifetime;
                opt.SlidingExpiration = tempOptions.Cookie.SlidingExpiration;
            });

        if (@this.Builder.Environment.IsEnvironment("Integration"))
        {
            @this.Builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.MapInboundClaims = false;

                    opt.TokenValidationParameters = new()
                    {
                        TokenDecryptionKey = new X509SecurityKey(encryption),

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new X509SecurityKey(signing),

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

        @this.Builder.Services.AddHttpContextAccessor();
        @this.Builder.Services.TryAddSingleton<IUserContext, UserContext>();

        @this.Builder.Services.TryAddScoped<IOAuth2PrincipalFactory, OAuth2PrincipalFactory>();

        @this.Builder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IClaimsPipelineStep<OAuth2PrincipalContext>), typeof(OAuth2SetSubjectStep), ServiceLifetime.Scoped));
        @this.Builder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IClaimsPipelineStep<OAuth2PrincipalContext>), typeof(OAuth2SetScopesStep), ServiceLifetime.Scoped));
        @this.Builder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IClaimsPipelineStep<OAuth2PrincipalContext>), typeof(OAuth2SetResourcesStep), ServiceLifetime.Scoped));
        @this.Builder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IClaimsPipelineStep<OAuth2PrincipalContext>), typeof(OAuth2SetClientMetadataStep), ServiceLifetime.Scoped));
        @this.Builder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IClaimsPipelineStep<OAuth2PrincipalContext>), typeof(OAuth2SetProtocolMetadataStep), ServiceLifetime.Scoped));
        @this.Builder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IClaimsPipelineStep<OAuth2PrincipalContext>), typeof(OAuth2ApplyDefaultDestinationsStep), ServiceLifetime.Scoped));

        var assemblies = tempOptions.ScanAssemblies;

        var allTypes = assemblies
            .SelectMany(assembly => { try { return assembly.GetTypes(); } catch { return []; } })
            .ToList();

        var userTypes = allTypes
            .Where(type => type.IsClass && !type.IsAbstract && !type.IsGenericType)
            .Where(type => type.IsAssignableTo(typeof(IUser)));

        foreach (var type in userTypes)
        {
            var method = typeof(IOAuth2BuilderExtensions)
                .GetMethod(nameof(AddUser), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(type);

            method.Invoke(null, [@this.Builder.Services, allTypes]);
        }

        return @this;
    }

    private static void AddUser<TUser>(
        IServiceCollection services,
        IEnumerable<Type> allTypes)
        where TUser : class, IUser
    {
        services.TryAddScoped<ICookiePrincipalFactory<TUser>, CookiePrincipalFactory<TUser>>();

        services.TryAddEnumerable(new ServiceDescriptor(typeof(IClaimsPipelineStep<CookiePrincipalContext<TUser>>), typeof(CookieSetSubjectStep<TUser>), ServiceLifetime.Scoped));
        services.TryAddEnumerable(new ServiceDescriptor(typeof(IClaimsPipelineStep<CookiePrincipalContext<TUser>>), typeof(CookieSetUserProfileStep<TUser>), ServiceLifetime.Scoped));

        services.TryAddEnumerable(new ServiceDescriptor(typeof(IClaimsPipelineStep<OAuth2PrincipalContext>), typeof(OAuth2SetUserProfileStep<TUser>), ServiceLifetime.Scoped));

        var storeTypes = allTypes
            .Where(type => type.IsClass && !type.IsAbstract && !type.IsGenericType)
            .Where(type => type.IsAssignableTo(typeof(IUserStore<TUser>)));

        foreach (var type in storeTypes)
        {
            services.TryAddScoped(typeof(IUserStore<TUser>), type);

            if (type.IsAssignableTo(typeof(IInternalUserStore<TUser>)))
                services.TryAddScoped(typeof(IInternalUserStore<TUser>), type);
        }
    }
}
