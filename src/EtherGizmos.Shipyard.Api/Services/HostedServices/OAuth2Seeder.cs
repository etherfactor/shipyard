using OpenIddict.Abstractions;
using System.Text.Json;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Shipyard.Api.Services.HostedServices;

public class OAuth2Seeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public OAuth2Seeder(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var provider = scope.ServiceProvider;

        await PopulateScopesAsync(provider, cancellationToken);
        await PopulateApplicationsAsync(provider, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    private async Task PopulateScopesAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        var scopeManager = serviceProvider.GetRequiredService<IOpenIddictScopeManager>();

        OpenIddictScopeDescriptor scope;

        scope = new()
        {
            Name = "app",
            DisplayName = "Entire App",
            Description = "Enables access to the entire application; will be subdivided later.",
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
        var applicationManager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        OpenIddictApplicationDescriptor application;

        application = new()
        {
            ClientId = "1c9cc927-68fe-4376-8d3f-b71ef15289b6",

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

                Permissions.Prefixes.Scope + "app",
            },
        };
        application.RedirectUris.Add(new Uri("https://localhost"));
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
