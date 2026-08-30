using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Shipyard.Controllers;

internal class JwtTokenMinter : ITokenMinter
{
    private readonly IKeyResolver _keyResolver;

    public JwtTokenMinter()
    {
        var services = new ServiceCollection();

        services.AddKeyResolver()
            .WithCertificates();

        var configuration = new ConfigurationManager();

        configuration.AddInMemoryCollection(new Dictionary<string, string?>()
        {
            ["Keys:AuthSigning:Type"] = "Asymmetric",
            ["Keys:AuthSigning:PfxFile:Path"] = Certificates.TokenSigningPath,
            ["Keys:AuthSigning:PfxFile:AutoGenerate"] = "true",
            ["Keys:AuthEncryption:Type"] = "Asymmetric",
            ["Keys:AuthEncryption:PfxFile:Path"] = Certificates.TokenEncryptionPath,
            ["Keys:AuthEncryption:PfxFile:AutoGenerate"] = "true",
        });

        services.AddSingleton<IConfiguration>(configuration);

        var provider = services.BuildServiceProvider();

        _keyResolver = provider.GetRequiredService<IKeyResolver>();
    }

    public string Mint(
        TokenRequest request)
    {
        List<Claim> claims =
        [
            new(Claims.Subject, request.Subject),
            .. request.Claims?.Select(e => new Claim(e.Key, e.Value)) ?? [],
        ];

        var signingCertificate = _keyResolver.LoadCertificate("AuthSigning");
        var signingCredentials = new X509SigningCredentials(signingCertificate);

        var encryptingCertificate = _keyResolver.LoadCertificate("AuthEncryption");
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
