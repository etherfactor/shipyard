using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers;

internal class JwtTokenMinter : ITokenMinter
{
    public string Mint(
        TokenRequest request)
    {
        List<Claim> claims =
        [
            new(Claims.Subject, request.Subject),
            .. request.Claims?.Select(e => new Claim(e.Key, e.Value)) ?? [],
        ];

        var signingCertificate = X509Certificate2.CreateFromPem(Certificates.TokenSigningPublicKey, Certificates.TokenSigningPrivateKey);
        var signingCredentials = new X509SigningCredentials(signingCertificate);

        var encryptingCertificate = X509Certificate2.CreateFromPem(Certificates.TokenEncryptionPublicKey, Certificates.TokenEncryptionPrivateKey);
        var encryptingCredentials = new X509EncryptingCredentials(encryptingCertificate);

        var handler = new JwtSecurityTokenHandler();

        var now = DateTime.UtcNow;
        var token = handler.CreateJwtSecurityToken(
            issuer: request.Issuer ?? "http://localhost",
            audience: request.Audience ?? "http://localhost",
            subject: new ClaimsIdentity(claims),
            notBefore: now,
            expires: request.Expires?.UtcDateTime ?? now.AddMinutes(15),
            issuedAt: now,
            signingCredentials: signingCredentials,
            encryptingCredentials: encryptingCredentials);

        var tokenContent = handler.WriteToken(token);

        return tokenContent;
    }
}
