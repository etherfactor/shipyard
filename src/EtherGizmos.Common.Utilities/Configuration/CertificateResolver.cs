using EtherGizmos.Common.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace EtherGizmos.Common.Configuration;

internal class CertificateResolver : ICertificateResolver
{
    private readonly IOptionsMonitor<Dictionary<string, CertificateOptions>> _options;
    private readonly IConfiguration _configuration;

    public CertificateResolver(
        IOptionsMonitor<Dictionary<string, CertificateOptions>> options,
        IConfiguration configuration)
    {
        _options = options;
        _configuration = configuration;
    }

    public OneOfCertificateReference GetCertificate(string certificateId)
    {
        var connection = GetConnection<CertificateReferenceOptions>(certificateId, CertificateType.Certificate);
        var result = connection switch
        {
            FileCertificateOptions file => (OneOfCertificateReference)file,
            TextCertificateOptions text => text,
            _ => connection
        };

        return result;
    }

    public X509Certificate2 LoadCertificate(string certificateId)
    {
        var certificate = GetCertificate(certificateId);
        return certificate.Match(
            _ => throw new InvalidOperationException($"The certificate {certificateId} is not a valid certificate."),
            file => LoadFromPfx(file),
            text => LoadFromRaw(text)
        );
    }

    private TOptions GetConnection<TOptions>(
        string connectionId,
        CertificateType expectedType)
        where TOptions : new()
    {
        var options = _options.CurrentValue;

        if (options.TryGetValue(connectionId, out var connection))
        {
            if (connection.Type == expectedType)
            {
                var properties = connection.GetType().GetProperties()
                    .Where(e => e.PropertyType.IsAssignableTo(typeof(TOptions)))
                    .Where(e => e.GetValue(connection) is not null)
                    .ToList();

                if (properties.Count == 1)
                {
                    return (TOptions)properties.Single().GetValue(connection)!;
                }
            }
        }

        return new TOptions();
    }

    private X509Certificate2 LoadFromPfx(
        FileCertificateOptions file)
    {
        if (!File.Exists(file.Path))
        {
            using var rsa = RSA.Create(2048);
            var request = new CertificateRequest(
                "CN=auto",
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            var now = DateTimeOffset.UtcNow;
            var signed = request.CreateSelfSigned(now, now.AddYears(100));

            var bytes = signed.Export(X509ContentType.Pfx, file.Password);
            File.WriteAllBytes(file.Path, bytes);
        }

        var certificate = X509CertificateLoader.LoadPkcs12FromFile(file.Path, file.Password);
        return certificate;
    }

    private X509Certificate2 LoadFromRaw(
        TextCertificateOptions text)
    {
        X509Certificate2.CreateFromPem(text.PublicKey);
        var certificate = X509Certificate2.CreateFromPem(text.PublicKey, text.PrivateKey);
        return certificate;
    }
}
