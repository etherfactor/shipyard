using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Database;
using OpenIddict.Abstractions;

namespace EtherGizmos.Shipyard.Api.Services.Pipeline.OAuth2;

public class OAuth2SetUserCapabilitiesStep : IClaimsPipelineStep<OAuth2PrincipalContext>
{
    public int Order => 300;

    private readonly IUserStore<User> _userStore;

    public OAuth2SetUserCapabilitiesStep(
        IUserStore<User> userStore)
    {
        _userStore = userStore;
    }

    public async Task<OAuth2PrincipalContext> ExecuteAsync(
        OAuth2PrincipalContext context,
        CancellationToken cancellationToken = default)
    {
        var subject = context.Subject.Value;
        var user = await _userStore.FindBySubjectAsync(subject, cancellationToken: cancellationToken);

        if (user is not null)
        {
            var capabilities = user.Capabilities;

            var securableTypeMap = capabilities
                .Where(e => e.IsAllowed == 1)
                .OrderBy(e => e.SecurableType)
                .GroupBy(e => e.SecurableType, e => e.PermissionId)
                .Select(group => new
                {
                    SecurableType = group.Key,
                    AggregatePermissions = group.Aggregate(0, (current, next) => current | next),
                })
                .ToDictionary(e => e.SecurableType, e => e.AggregatePermissions);

            var capabilitiesStr = string.Join(";", securableTypeMap.Select(e => $"{e.Key}:{e.Value}"));
            context.Identity.SetClaim("cap", capabilitiesStr);
        }

        return context;
    }
}
