using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using System.Text.Json;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Shipyard.Services.Bootstrappers;

internal class OAuth2Bootstrapper : IBootstrapper
{
    public int Order => 200;

    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWorkFactory _uowFactory;

    public OAuth2Bootstrapper(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        IUnitOfWorkFactory uowFactory)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _uowFactory = uowFactory.AsUnfiltered();
    }

    public async Task ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var provider = scope.ServiceProvider;

        await PopulateScopesAsync(provider, cancellationToken);
        await PopulateApplicationsAsync(provider, cancellationToken);
    }

    private async Task PopulateScopesAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        var scopeManager = serviceProvider.GetRequiredService<IOpenIddictScopeManager>();

        OpenIddictScopeDescriptor scope;

        //Legacy scopes
        scope = new()
        {
            Name = AppConstants.Scopes.EntireApp,
            DisplayName = "Entire App",
            Description = "Enables access to the entire application; will be subdivided later.",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(false),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        //Carrier scopes
        scope = new()
        {
            Name = AppConstants.Scopes.CarrierRead,
            DisplayName = "View Carriers",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        scope = new()
        {
            Name = AppConstants.Scopes.CarrierWrite,
            DisplayName = "Modify Carriers",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        scope = new()
        {
            Name = AppConstants.Scopes.CarrierDelete,
            DisplayName = "Delete Carriers",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        //Carrier execution scopes
        scope = new()
        {
            Name = AppConstants.Scopes.CarrierExecutionRead,
            DisplayName = "View Carrier Executions",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        //Group scopes
        scope = new()
        {
            Name = AppConstants.Scopes.GroupRead,
            DisplayName = "View Groups",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        scope = new()
        {
            Name = AppConstants.Scopes.GroupWrite,
            DisplayName = "Modify Groups",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        scope = new()
        {
            Name = AppConstants.Scopes.GroupDelete,
            DisplayName = "Delete Groups",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        //Package scopes
        scope = new()
        {
            Name = AppConstants.Scopes.PackageRead,
            DisplayName = "View Packages",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        scope = new()
        {
            Name = AppConstants.Scopes.PackageWrite,
            DisplayName = "Modify Packages",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        scope = new()
        {
            Name = AppConstants.Scopes.PackageDelete,
            DisplayName = "Delete Packages",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        //Role scopes
        scope = new()
        {
            Name = AppConstants.Scopes.RoleRead,
            DisplayName = "View Roles",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        //Tracking update scopes
        scope = new()
        {
            Name = AppConstants.Scopes.TrackingUpdateRead,
            DisplayName = "View Tracking Updates",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        //User scopes
        scope = new()
        {
            Name = AppConstants.Scopes.UserRead,
            DisplayName = "View Users",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        scope = new()
        {
            Name = AppConstants.Scopes.UserWrite,
            DisplayName = "Modify Users",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);

        scope = new()
        {
            Name = AppConstants.Scopes.UserDelete,
            DisplayName = "Delete Users",
            Description = "...",
            Properties =
            {
                ["is_public"] = JsonSerializer.SerializeToElement(true),
            },
        };
        await CreateOrUpdateScopeAsync(scopeManager, scope, cancellationToken);
    }

    private async Task CreateOrUpdateScopeAsync(
        IOpenIddictScopeManager scopeManager,
        OpenIddictScopeDescriptor scope,
        CancellationToken cancellationToken = default)
    {
        var scopeInstance = await scopeManager.FindByNameAsync(scope.Name!, cancellationToken);
        if (scopeInstance is not null)
        {
            await scopeManager.UpdateAsync(scopeInstance, scope, cancellationToken);
        }
        else
        {
            await scopeManager.CreateAsync(scope, cancellationToken);
        }
    }

    private async Task PopulateApplicationsAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create();
        var userRepo = uow.Repository<User>();

        var applicationManager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        OpenIddictApplicationDescriptor application;

        //Create the web UI application
        application = new()
        {
            ClientId = AppConstants.Applications.WebUIClientId.ToString(),

            DisplayName = "Shipyard Web UI",

            ClientType = ClientTypes.Public,
            ConsentType = ConsentTypes.Implicit,

            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
                Permissions.Endpoints.Revocation,
                Permissions.Endpoints.Authorization,

                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,

                Permissions.ResponseTypes.Code,

                Permissions.Prefixes.Scope + AppConstants.Scopes.EntireApp,
            },
        };

        var urls = _configuration
            .GetSection("Security:OAuth2:WebUIRedirectUrls")
            .Get<string[]>() ?? [];

        foreach (var url in urls)
        {
            application.RedirectUris.Add(new Uri(url));
        }

        await CreateOrUpdateApplicationAsync(applicationManager, application, cancellationToken);

        //Create the worker application
        var worker = await userRepo.Data.SingleAsync(e => e.SystemId == AppConstants.Users.WorkerSystemId, cancellationToken: cancellationToken);
        application = new()
        {
            ClientId = AppConstants.Applications.WorkerClientId.ToString(),
            ClientSecret = _configuration["Security:OAuth2:WorkerClientSecret"],

            DisplayName = "Shipyard Worker",

            ClientType = ClientTypes.Confidential,
            ConsentType = ConsentTypes.Implicit,

            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
                Permissions.Endpoints.Revocation,

                Permissions.GrantTypes.ClientCredentials,

                Permissions.Prefixes.Scope + AppConstants.Scopes.CarrierRead,
                //Permissions.Prefixes.Scope + AppConstants.Scopes.CarrierWrite,
                //Permissions.Prefixes.Scope + AppConstants.Scopes.CarrierDelete,
                //Permissions.Prefixes.Scope + AppConstants.Scopes.CarrierExecutionRead,
                //Permissions.Prefixes.Scope + AppConstants.Scopes.GroupRead,
                //Permissions.Prefixes.Scope + AppConstants.Scopes.GroupWrite,
                //Permissions.Prefixes.Scope + AppConstants.Scopes.GroupDelete,
                //Permissions.Prefixes.Scope + AppConstants.Scopes.PackageRead,
                Permissions.Prefixes.Scope + AppConstants.Scopes.PackageWrite,
                //Permissions.Prefixes.Scope + AppConstants.Scopes.PackageDelete,
                //Permissions.Prefixes.Scope + AppConstants.Scopes.RoleRead,
                Permissions.Prefixes.Scope + AppConstants.Scopes.TrackingUpdateRead,
                //Permissions.Prefixes.Scope + AppConstants.Scopes.UserRead,
                //Permissions.Prefixes.Scope + AppConstants.Scopes.UserWrite,
                //Permissions.Prefixes.Scope + AppConstants.Scopes.UserDelete,
            },

            Properties =
            {
                ["user_id"] = JsonSerializer.SerializeToElement(worker.Id),
            },
        };

        await CreateOrUpdateApplicationAsync(applicationManager, application, cancellationToken);
    }

    private async Task CreateOrUpdateApplicationAsync(
        IOpenIddictApplicationManager applicationManager,
        OpenIddictApplicationDescriptor application,
        CancellationToken cancellationToken)
    {
        var applicationInstance = await applicationManager.FindByClientIdAsync(application.ClientId!, cancellationToken);
        if (applicationInstance is not null)
        {
            await applicationManager.UpdateAsync(applicationInstance, application, cancellationToken);
        }
        else
        {
            await applicationManager.CreateAsync(application, cancellationToken);
        }
    }
}
