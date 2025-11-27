using EtherGizmos.Common.Abstractions;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Services.Pipeline.OAuth2;

internal class OAuth2SetSubjectStep : IClaimsPipelineStep<OAuth2PrincipalContext>
{
    public int Order => -999;

    public Task<OAuth2PrincipalContext> ExecuteAsync(
        OAuth2PrincipalContext context,
        CancellationToken cancellationToken = default)
    {
        var subject = context.Subject.Value;
        context.Identity.SetClaim(Claims.Subject, subject);
        context.Identity.SetClaim("sub_kind", OAuth2SubjectKindConverter.ToString(context.Subject.Kind));

        return Task.FromResult(context);
    }
}
