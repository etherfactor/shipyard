using EtherGizmos.Common.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace EtherGizmos.Common.Abstractions;

public interface ICertificateResolver
{
    OneOfCertificateReference GetCertificate(string certificateId);

    X509Certificate2 LoadCertificate(string certificateId);
}
