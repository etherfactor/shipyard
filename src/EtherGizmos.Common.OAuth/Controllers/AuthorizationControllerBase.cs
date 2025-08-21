using EtherGizmos.Common.Extensions;
using EtherGizmos.Common.ViewModels;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Collections.Immutable;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route(AuthorizationConstants.OAuth2.ControllerPath)]
public abstract class AuthorizationControllerBase : Controller
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly IOpenIddictScopeManager _scopeManager;

    protected virtual string ServerScheme => OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;

    protected virtual string LoginScheme => AuthorizationConstants.Cookie.AuthenticationScheme;

    public AuthorizationControllerBase(
        IServiceProvider serviceProvider)
    {
        _applicationManager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        _authorizationManager = serviceProvider.GetRequiredService<IOpenIddictAuthorizationManager>();
        _scopeManager = serviceProvider.GetRequiredService<IOpenIddictScopeManager>();
    }

    [IgnoreAntiforgeryToken]
    [HttpGet(AuthorizationConstants.OAuth2.AuthorizePath)]
    public virtual async Task<IActionResult> Authorize(
        CancellationToken cancellationToken)
    {
        var result = await LoadOAuth2Async(cancellationToken);
        if (result.Error is not null)
            return result.Error;

        var clientId = result.ClientId;
        var application = result.Application;
        var requestedScopes = result.Scopes.OrderBy(e => e).ToArray();

        var hasPromptLogin = result.Prompts.Contains(PromptValues.Login);
        var hasPromptNone = result.Prompts.Contains(PromptValues.None);
        var hasPromptConsent = result.Prompts.Contains(PromptValues.Consent);

        var applicationId = (await _applicationManager.GetIdAsync(application, cancellationToken))!;
        var applicationName = (await _applicationManager.GetDisplayNameAsync(application, cancellationToken))!;
        var applicationPermissions = await _applicationManager.GetPermissionsAsync(application, cancellationToken);

        var applicationScopes = applicationPermissions
            .Where(e => e.StartsWith(Permissions.Prefixes.Scope))
            .Select(e => e.Substring(Permissions.Prefixes.Scope.Length))
            .OrderBy(e => e)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var allowedScopes = (await _applicationManager.GetPermissionsAsync(application, cancellationToken))
            .Where(p => p.StartsWith(Permissions.Prefixes.Scope))
            .Select(p => p.Substring(Permissions.Prefixes.Scope.Length))
            .Append("openid")
            .Append("offline_access")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (!requestedScopes.All(scope => allowedScopes.Contains(scope, StringComparer.OrdinalIgnoreCase)))
            return ForbidWithError(Errors.InvalidScope, "One or more scopes are not permitted for this client.");

        var userId = result.Principal!.GetClaim(Claims.Subject)?.ToLower();
        if (userId is null)
            return TryChallenge(hasPromptNone);

        var consentType = await _applicationManager.GetConsentTypeAsync(application, cancellationToken);

        var consentIsImplicit = consentType is ConsentTypes.Implicit or ConsentTypes.External;
        var consentIsForced = !consentIsImplicit && hasPromptConsent;

        var existingAuthorization = consentIsImplicit
            ? null
            : await FindExistingAuthorizationAsync(userId, clientId, requestedScopes, cancellationToken);

        if (consentIsImplicit || (!consentIsForced && existingAuthorization is not null))
        {
            var identity = await CreateOAuth2PrincipalAsync(
                user: result.Principal!,
                request: result.Request,
                applicationName: applicationName,
                scopes: requestedScopes,
                cancellationToken: cancellationToken);

            identity.SetScopes(requestedScopes);

            if (existingAuthorization is not null)
                identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(existingAuthorization, cancellationToken));

            identity.SetDestinations(GetDestinations);
            return SignIn(new ClaimsPrincipal(identity), ServerScheme);
        }

        if (hasPromptNone)
            return ForbidWithError(Errors.ConsentRequired, "User consent is required, but prompt=none was specified.");

        var viewModel = await BuildConsentViewModelAsync(applicationName, clientId, requestedScopes, cancellationToken);
        return View("Authorize", viewModel);
    }

    [ValidateAntiForgeryToken]
    [FormValueRequired("submit.Accept")]
    [HttpPost(AuthorizationConstants.OAuth2.AuthorizePath)]
    public virtual async Task<IActionResult> Accept(
        ConsentViewModel model,
        CancellationToken cancellationToken)
    {
        var result = await LoadOAuth2Async(cancellationToken);
        if (result.Error is not null)
            return result.Error;

        var clientId = result.ClientId;
        var application = result.Application;

        var applicationId = (await _applicationManager.GetIdAsync(application, cancellationToken))!;
        var applicationName = (await _applicationManager.GetDisplayNameAsync(application, cancellationToken))!;

        var acceptedScopes = model.Scopes
            .Where(e => e.IsApproved)
            .Select(e => e.Name)
            .ToImmutableArray();

        var principal = await CreateOAuth2PrincipalAsync(
            user: result.Principal!,
            request: result.Request,
            applicationName: applicationName,
            scopes: acceptedScopes,
            cancellationToken: cancellationToken);

        principal.SetScopes(acceptedScopes);
        principal.SetDestinations(GetDestinations);

        var consentType = await _applicationManager.GetConsentTypeAsync(application, cancellationToken);
        var subject = result.Principal!.GetClaim(Claims.Subject)!;

        if (consentType is ConsentTypes.Explicit or ConsentTypes.Systematic)
        {
            if (consentType is ConsentTypes.Explicit)
            {
                var existing = await FindExistingAuthorizationAsync(subject, clientId, acceptedScopes, cancellationToken);
                var authorization = existing ?? await _authorizationManager.CreateAsync(
                    principal: principal,
                    subject: subject,
                    client: clientId,
                    type: AuthorizationTypes.Permanent,
                    scopes: acceptedScopes,
                    cancellationToken: cancellationToken);

                principal.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization, cancellationToken));
            }
        }

        return SignIn(principal, ServerScheme);
    }

    [ValidateAntiForgeryToken]
    [FormValueRequired("submit.Deny")]
    [HttpPost(AuthorizationConstants.OAuth2.AuthorizePath)]
    public virtual Task<IActionResult> Deny(
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            ForbidWithError(Errors.AccessDenied, "User denied consent."));
    }

    [HttpPost(AuthorizationConstants.OAuth2.TokenPath)]
    public virtual async Task<IActionResult> Token(
        CancellationToken cancellationToken)
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("No OpenIddict request could be retrieved.");

        if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
        {
            var result = await HttpContext.AuthenticateAsync(ServerScheme);
            if (!result.Succeeded)
                return Forbid(ServerScheme);

            var principal = result.Principal!;
            await EnrichOAuth2PrincipalAsync(principal, request, cancellationToken);
            return SignIn(principal, ServerScheme);
        }

        return ForbidWithError(Errors.UnsupportedGrantType, "This grant type is not supported.");
    }

    protected virtual async Task<OAuth2Result> LoadOAuth2Async(
        CancellationToken cancellationToken = default)
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("No OpenIddict request could be retrieved.");

        var result = await HttpContext.AuthenticateAsync(LoginScheme);

        var hasPromptLogin = request.HasPromptValue(PromptValues.Login);
        var hasPromptNone = request.HasPromptValue(PromptValues.None);
        var hasPromptConsent = request.HasPromptValue(PromptValues.Consent);

        if (hasPromptLogin || !(result?.Principal?.Identity?.IsAuthenticated ?? false))
            return new() { Error = TryChallenge(hasPromptNone) };

        HttpContext.User = result!.Principal!;

        var clientId = request.ClientId;
        if (!Guid.TryParse(clientId, out var clientIdGuid))
            return new() { Error = ForbidWithError(Errors.InvalidClient, $"The client id '{clientId}' is not valid.") };

        clientId = clientIdGuid.ToString().ToLower();

        var application = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken);
        if (application is null)
            return new() { Error = ForbidWithError(Errors.InvalidClient, $"The client id '{clientId}' is not valid.") };

        return new()
        {
            Request = request,
            Principal = result.Principal,
            Application = application,
            ClientId = clientId,
            Prompts = request.GetPromptValues(),
            Scopes = request.GetScopes(),
        };
    }

    protected virtual IActionResult TryChallenge(
        bool hasPromptNone)
    {
        if (!hasPromptNone)
        {
            var request = HttpContext.GetOpenIddictServerRequest()!;

            var prompts = string.Join(" ", request.GetPromptValues().Remove(PromptValues.Login));

            var parameters = Request.HasFormContentType
                ? Request.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList()
                : Request.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();

            parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompts)));

            return Challenge(
                authenticationSchemes: LoginScheme,
                properties: new()
                {
                    RedirectUri = Request.PathBase + Request.Path
                        + QueryString.Create(parameters)
                });
        }
        else
        {
            return ForbidWithError(Errors.LoginRequired, "The user is not logged in, and prompt=none was specified.");
        }
    }

    protected virtual IActionResult ForbidWithError(
        string errorCode,
        string errorMessage)
    {
        return Forbid(
            authenticationSchemes: ServerScheme,
            properties: new(new Dictionary<string, string?>()
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = errorCode,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = errorMessage,
            }));
    }

    protected virtual async Task<object?> FindExistingAuthorizationAsync(
        string subject, string clientId, IEnumerable<string> scopes, CancellationToken cancellationToken)
    {
        await foreach (var authorization in _authorizationManager.FindAsync(
            subject: subject,
            client: clientId,
            status: Statuses.Valid,
            type: AuthorizationTypes.Permanent,
            scopes: ImmutableArray.Create(scopes.ToArray()),
            cancellationToken: cancellationToken))
        {
            if (authorization is not null)
                return authorization;
        }

        return null;
    }

    protected virtual Task<ClaimsPrincipal> CreateOAuth2PrincipalAsync(
        ClaimsPrincipal user,
        OpenIddictRequest request,
        string applicationName,
        IEnumerable<string> scopes,
        CancellationToken cancellationToken = default)
    {
        var identity = new ClaimsIdentity(
            authenticationType: ServerScheme,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.AddClaim(Claims.Subject, user.GetClaim(Claims.Subject)!);
        identity.TryAddClaim(Claims.Name, user.GetClaim(Claims.Name));
        identity.TryAddClaim(Claims.GivenName, user.GetClaim(Claims.GivenName));
        identity.TryAddClaim(Claims.FamilyName, user.GetClaim(Claims.FamilyName));
        identity.TryAddClaim(Claims.Email, user.GetClaim(Claims.Email));
        identity.TryAddClaim("client_name", applicationName);

        identity.SetScopes(scopes);

        //identity.SetAuthorizationId(user.GetAuthorizationId());

        identity.SetDestinations(GetDestinations);

        var principal = new ClaimsPrincipal(identity);
        return Task.FromResult(principal);
    }

    protected virtual Task EnrichOAuth2PrincipalAsync(
        ClaimsPrincipal principal,
        OpenIddictRequest request,
        CancellationToken ct)
        => Task.CompletedTask;

    protected virtual IEnumerable<string> GetDestinations(Claim claim)
    {
        switch (claim.Type)
        {
            case Claims.Name:
            case Claims.Email:
                yield return Destinations.AccessToken;
                yield return Destinations.IdentityToken;
                yield break;
        }

        yield return Destinations.AccessToken;
    }

    protected virtual async Task<ConsentViewModel> BuildConsentViewModelAsync(
        string applicationName,
        string clientId,
        IReadOnlyCollection<string> requestedScopes,
        CancellationToken ct)
    {
        var scopes = new List<ConsentScopeViewModel>();

        foreach (var scopeName in requestedScopes)
        {
            var scope = await _scopeManager.FindByNameAsync(scopeName, ct);
            var displayName = scope is null
                ? scopeName
                : await _scopeManager.GetDisplayNameAsync(scope, ct) ?? scopeName;

            var description = scope is null
                ? null
                : await _scopeManager.GetDescriptionAsync(scope, ct);

            scopes.Add(new()
            {
                Name = scopeName,
                DisplayName = displayName,
                Description = description
            });
        }

        return new()
        {
            ApplicationName = applicationName,
            ClientId = clientId,
            Scopes = scopes,
        };
    }

    protected sealed record OAuth2Result
    {
        public OpenIddictRequest Request { get; init; } = null!;

        public ClaimsPrincipal Principal { get; init; } = null!;

        public IActionResult? Error { get; init; }

        public object Application { get; init; } = null!;

        public string ClientId { get; init; } = null!;

        public ImmutableArray<string> Prompts { get; init; }

        public ImmutableArray<string> Scopes { get; init; }
    }
}
