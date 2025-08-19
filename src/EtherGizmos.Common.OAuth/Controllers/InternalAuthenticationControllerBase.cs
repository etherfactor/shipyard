using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.ViewModels;
using EtherGizmos.Shipyard.Api.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route(AuthorizationConstants.Cookie.ControllerPath)]
public abstract class InternalAuthenticationControllerBase<TUser> : Controller
    where TUser : class, IInternalUser
{
    protected virtual string LoginScheme => CookieAuthenticationDefaults.AuthenticationScheme;

    public InternalAuthenticationControllerBase(
        IServiceProvider serviceProvider)
    {

    }

    [IgnoreAntiforgeryToken]
    [HttpGet(AuthorizationConstants.Cookie.LoginPath)]
    public virtual Task<IActionResult> Login(
        [FromQuery(Name = AuthorizationConstants.Cookie.ReturnUrlParameter)]
        string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        var viewModel = new LoginViewModel()
        {
            ReturnUrl = returnUrl,
        };

        return Task.FromResult<IActionResult>(
            View("Login", viewModel));
    }

    [ValidateAntiForgeryToken]
    [HttpPost(AuthorizationConstants.Cookie.LoginPath)]
    public virtual async Task<IActionResult> Login(
        LoginViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (ModelState.IsValid)
        {
            var user = await FindUserAsync(model.Username, cancellationToken);
            if (user is not null)
            {
                var validated = await ValidatePasswordAsync(user, model.Password, cancellationToken);
                if (validated)
                {
                    var principal = await CreateCookiePrincipalAsync(user, cancellationToken);

                    await HttpContext.SignInAsync(LoginScheme, principal);
                    await OnLoginAsync(user, cancellationToken);

                    return LocalRedirect(model.ReturnUrl ?? "/");
                }
            }
        }

        ModelState.AddModelError(nameof(model.Password), "Invalid username or password.");
        model.Password = null!;

        return View("Login", model);
    }

    [IgnoreAntiforgeryToken]
    [HttpGet(AuthorizationConstants.Cookie.LogoutPath)]
    public virtual async Task<IActionResult> Logout(
        [FromQuery(Name = AuthorizationConstants.Cookie.ReturnUrlParameter)]
        string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        await HttpContext.SignOutAsync(LoginScheme);
        return LocalRedirect(returnUrl ?? "/");
    }

    protected abstract Task<TUser?> FindUserAsync(
        string username,
        CancellationToken cancellationToken = default);

    protected abstract string FindSubject(
        TUser user);

    protected abstract Task<bool> ValidatePasswordAsync(
        TUser user,
        string password,
        CancellationToken cancellationToken = default);

    protected virtual Task<ClaimsPrincipal> CreateCookiePrincipalAsync(
        TUser user,
        CancellationToken cancellationToken = default)
    {
        var identity = new ClaimsIdentity(
            authenticationType: LoginScheme,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.AddClaim(Claims.Subject, FindSubject(user));
        identity.TryAddClaim(Claims.Name, user.FullName);
        identity.TryAddClaim(Claims.GivenName, user.GivenName);
        identity.TryAddClaim(Claims.FamilyName, user.FamilyName);
        identity.TryAddClaim(Claims.Email, user.EmailAddress);

        var principal = new ClaimsPrincipal(identity);
        return Task.FromResult(principal);
    }

    protected virtual Task OnLoginAsync(
        TUser user,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
