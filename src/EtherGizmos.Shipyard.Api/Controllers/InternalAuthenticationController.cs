using EtherGizmos.Common;
using EtherGizmos.Common.Controllers;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.ViewModels;
using EtherGizmos.Shipyard.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Shipyard.Api.Controllers;

public class InternalAuthenticationController : InternalAuthenticationControllerBase<User>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    public InternalAuthenticationController(
        IServiceProvider serviceProvider,
        IUnitOfWorkFactory uowFactory)
        : base(serviceProvider)
    {
        _uowFactory = uowFactory;
    }

    [HttpGet("change-password")]
    [Authorize(AuthenticationSchemes = AuthorizationConstants.Cookie.AuthenticationScheme)]
    public IActionResult ChangePassword(
        [FromQuery] string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        return View(nameof(ChangePassword), new ChangePasswordViewModel()
        {
            ReturnUrl = returnUrl,
        });
    }

    [HttpPost("change-password")]
    [Authorize(AuthenticationSchemes = AuthorizationConstants.Cookie.AuthenticationScheme)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePasswordPost(
        ChangePasswordViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(ChangePassword), model);
        }

        using var uow = _uowFactory.Create(useRequestScope: true);
        var userRepo = uow.Repository<User>();

        var subject = Guid.Parse(User.GetClaim(Claims.Subject)!);
        var user = await userRepo.Data.SingleAsync(e => e.Id == subject, cancellationToken: cancellationToken);

        var matches = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword) == PasswordVerificationResult.Success;
        if (!matches)
        {
            ModelState.AddModelError(nameof(model.CurrentPassword), "Invalid password");
            return View(nameof(ChangePassword), model);
        }

        if (model.NewPassword != model.ConfirmPassword)
        {
            ModelState.AddModelError(nameof(model.ConfirmPassword), "Passwords must match");
            return View(nameof(ChangePassword), model);
        }

        user.Password = model.NewPassword;

        await uow.SaveChangesAsync(cancellationToken);

        return Redirect(model.ReturnUrl ?? "/");
    }
}
