using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Shipyard.Api.Services.Security;

public class HasCapabilityFilter : IAsyncAuthorizationFilter
{
    private readonly SecurableType _securableType;
    private readonly int _permissionId;

    public HasCapabilityFilter(
        SecurableType securableType,
        int permissionId)
    {
        _securableType = securableType;
        _permissionId = permissionId;
    }

    public async Task OnAuthorizationAsync(
        AuthorizationFilterContext context)
    {
        var httpUser = context.HttpContext.User;
        if (httpUser is not null)
        {
            var services = context.HttpContext.RequestServices;
            var uowFactory = services.GetRequiredService<IUnitOfWorkFactory>();

            using var uow = uowFactory.Create(context.HttpContext.RequestServices);
            var userRepo = uow.Repository<User>();

            var subject = httpUser.GetClaim(Claims.Subject);
            if (Guid.TryParse(subject, out var userId))
            {
                var user = await userRepo.Data
                    .SingleOrDefaultAsync(e => e.Id == userId);

                if (user is not null)
                {
                    if (user.Capabilities.Any(e => e.SecurableType == _securableType && e.PermissionId == _permissionId))
                    {
                        //The user has the capability
                        return;
                    }
                }
            }
        }

        //The user does not exist or does not have the capability
        context.Result = new ForbidResult();
    }
}

public class HasCapabilityAttribute : TypeFilterAttribute
{
    public SecurableType SecurableType { get; }

    public int PermissionId { get; }

    public HasCapabilityAttribute(
        SecurableType securableType,
        int permissionId)
        : base(typeof(HasCapabilityFilter))
    {
        SecurableType = securableType;
        PermissionId = permissionId;

        Arguments = [SecurableType, PermissionId];
    }
}
