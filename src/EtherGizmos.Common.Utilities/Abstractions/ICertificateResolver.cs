using System.Security.Cryptography.X509Certificates;

namespace EtherGizmos.Common.Abstractions;

public interface ICertificateResolver
{
    X509Certificate2 GetCertificate(string certificateId);
}
