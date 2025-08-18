using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    protected virtual string ServerScheme => OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;

    protected virtual string LoginScheme => AuthorizationConstants.Cookie.AuthenticationScheme;

    public AuthorizationControllerBase(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager)
    {
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
    }

    [IgnoreAntiforgeryToken]
    [HttpGet(AuthorizationConstants.OAuth2.AuthorizePath)]
    public virtual async Task<IActionResult> Authorize(
        CancellationToken cancellationToken)
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("No OpenIddict request could be retrieved.");

        var result = await HttpContext.AuthenticateAsync(LoginScheme);

        var hasPromptLogin = request.HasPromptValue(PromptValues.Login);
        var hasPromptNone = request.HasPromptValue(PromptValues.None);
        var hasPromptConsent = request.HasPromptValue(PromptValues.Consent);

        if (hasPromptLogin || !(result?.Principal?.Identity?.IsAuthenticated ?? false))
            TryChallenge(hasPromptNone);

        HttpContext.User = result!.Principal!;

        var clientId = request.ClientId;
        if (!Guid.TryParse(clientId, out var clientIdGuid))
            return ForbidWithError($"The client id '{clientId}' is not valid.");

        clientId = clientIdGuid.ToString().ToLower();

        var application = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken);
        if (application is null)
            return ForbidWithError($"The client id '{clientId}' is not valid.");

        var applicationId = (await _applicationManager.GetIdAsync(application, cancellationToken))!;

        var applicationName = (await _applicationManager.GetDisplayNameAsync(application, cancellationToken))!;

        var applicationPermissions = await _applicationManager.GetPermissionsAsync(application, cancellationToken);

        var applicationScopes = applicationPermissions
            .Where(e => e.StartsWith(Permissions.Prefixes.Scope))
            .Select(e => e.Substring(Permissions.Prefixes.Scope.Length))
            .OrderBy(e => e)
            .ToList();

        var userId = result.Principal!.GetClaim(Claims.Subject)?.ToLower();
        if (userId is null)
            return TryChallenge(hasPromptNone);

        var consentType = await _applicationManager.GetConsentTypeAsync(application, cancellationToken);

        var consentIsImplicit = consentType is ConsentTypes.Implicit or ConsentTypes.External;
        var consentIsForced = !consentIsImplicit && hasPromptConsent;

        var requestedScopes = request.GetScopes().OrderBy(e => e).ToArray();

        var existingAuthorization = consentIsImplicit
            ? null
            : FindExistingAuthorizationAsync(userId, clientId, requestedScopes, cancellationToken);
        if (consentIsImplicit || (!consentIsForced && existingAuthorization is not null))
        {
            var identity = await CreateUserPrincipalAsync(result.Principal!, request, cancellationToken);
            identity.SetScopes(requestedScopes);

            if (existingAuthorization is not null)
                identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(existingAuthorization, cancellationToken));

            identity.SetDestinations(GetDestinations);
            return SignIn(new ClaimsPrincipal(identity), ServerScheme);
        }

        if (hasPromptNone)
            return ForbidWithError("User consent is required, but prompt=none was specified.");

        var viewModel = await BuildConsentViewModelAsync();
        return View(viewModel);
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
                authenticationSchemes: ServerScheme,
                properties: new()
                {
                    RedirectUri = Request.PathBase + Request.Path
                        + QueryString.Create(parameters)
                });
        }
        else
        {
            return ForbidWithError("The user is not logged in, and prompt=none was specified.");
        }
    }

    protected virtual IActionResult ForbidWithError(
        string errorMessage)
    {
        return Forbid(
            authenticationSchemes: ServerScheme,
            properties: new(new Dictionary<string, string?>()
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = errorMessage,
            }));
    }

    [ValidateAntiForgeryToken]
    [FormValueRequired("submit.Accept")]
    [HttpPost(AuthorizationConstants.OAuth2.AuthorizePath)]
    public virtual async Task<IActionResult> Accept(
        CancellationToken cancellationToken)
    {
        
    }

    [ValidateAntiForgeryToken]
    [FormValueRequired("submit.Deny")]
    [HttpPost(AuthorizationConstants.OAuth2.AuthorizePath)]
    public virtual Task<IActionResult> Deny(
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IActionResult>(
            Forbid(ServerScheme));
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
                Forbid(ServerScheme);

            var principal = result.Principal!;
            await EnrichPrincipalAsync(principal, request, cancellationToken);
            return SignIn(principal, ServerScheme);
        }

        return Forbid(ServerScheme);
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

    protected abstract Task<ClaimsPrincipal> CreateUserPrincipalAsync(
        ClaimsPrincipal user, OpenIddictRequest request, CancellationToken ct);

    protected virtual Task EnrichPrincipalAsync(ClaimsPrincipal principal, OpenIddictRequest request, CancellationToken ct)
        => Task.CompletedTask;

    protected virtual IEnumerable<string> GetDestinations(Claim claim)
    {
        switch (claim.Type)
        {
            case OpenIddictConstants.Claims.Name:
            case OpenIddictConstants.Claims.Email:
                yield return OpenIddictConstants.Destinations.AccessToken;
                yield return OpenIddictConstants.Destinations.IdentityToken;
                yield break;
        }

        yield return OpenIddictConstants.Destinations.AccessToken;
    }
}
