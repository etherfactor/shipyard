using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using System.Security.Claims;

namespace EtherGizmos.Common.Abstractions;

public record OAuth2PrincipalContext(
    HttpContext HttpContext,
    OAuth2Subject Subject,
    OAuth2Request Request,
    UpstreamPrincipal UpstreamPrincipal)
    : IClaimsContext
{
    public ClaimsIdentity Identity { get; init; } = new();

    public static OAuth2PrincipalContext FromAuthorizeEndpoint(
        HttpContext httpContext,
        OpenIddictRequest request,
        ClaimsPrincipal loginPrincipal)
        => FromRawData(httpContext, request, loginPrincipal,
            subjectKind: OAuth2SubjectKind.User,
            currentGrantKind: OAuth2GrantKind.AuthorizationCode);

    public static OAuth2PrincipalContext FromAuthorizationCode(
        HttpContext httpContext,
        OpenIddictRequest request,
        ClaimsPrincipal codePrincipal)
        => FromRawData(httpContext, request, codePrincipal,
            subjectKind: OAuth2SubjectKind.User,
            currentGrantKind: OAuth2GrantKind.AuthorizationCode);

    public static OAuth2PrincipalContext FromRefreshToken(
        HttpContext httpContext,
        OpenIddictRequest request,
        ClaimsPrincipal refreshPrincipal)
        => FromRawData(httpContext, request, refreshPrincipal,
            currentGrantKind: OAuth2GrantKind.RefreshToken);

    public static OAuth2PrincipalContext FromClientCredentials(
        HttpContext httpContext,
        OpenIddictRequest request,
        ClaimsPrincipal clientPrincipal)
        => FromRawData(httpContext, request, clientPrincipal,
            subjectKind: OAuth2SubjectKind.Client,
            currentGrantKind: OAuth2GrantKind.ClientCredentials);

    private static OAuth2PrincipalContext FromRawData(
        HttpContext httpContext,
        OpenIddictRequest request,
        ClaimsPrincipal principal,
        OAuth2SubjectKind? subjectKind = null,
        OAuth2GrantKind? currentGrantKind = null,
        OAuth2GrantKind? originalGrantKind = null)
    {
        var useSubjectKind = subjectKind
            ?? OAuth2SubjectKindConverter.FromStringOrDefault(principal.GetClaim("sub_kind"))
            ?? OAuth2SubjectKind.Unknown;
        var useCurrentGrantKind = currentGrantKind
            ?? OAuth2GrantKindConverter.FromStringOrDefault(principal.GetClaim("gty"))
            ?? OAuth2GrantKind.Unknown;
        var useOriginalGrantKind = originalGrantKind
            ?? OAuth2GrantKindConverter.FromStringOrDefault(principal.GetClaim("gty_init"))
            ?? useCurrentGrantKind;

        var cmpSubject = OAuth2Subject.Create(
            kind: useSubjectKind,
            value: principal.GetClaim(OpenIddictConstants.Claims.Subject)
                ?? throw new InvalidOperationException("The principal must have a 'sub' claim"));

        var cmpRequest = OAuth2Request.Create(
            openIddict: request,
            currentGrant: useCurrentGrantKind,
            originalGrant: useOriginalGrantKind);

        var cmpPrincipal = UpstreamPrincipal.Create(
            principal: principal);

        return new OAuth2PrincipalContext(
            HttpContext: httpContext,
            Subject: cmpSubject,
            Request: cmpRequest,
            UpstreamPrincipal: cmpPrincipal);
    }
}
