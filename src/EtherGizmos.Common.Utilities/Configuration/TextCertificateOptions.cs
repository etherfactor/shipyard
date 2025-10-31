using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Common.Configuration;

public class TextCertificateOptions : CertificateReferenceOptions
{
    [Required]
    public string PublicKey { get; set; } = null!;

    [Required]
    public string PrivateKey { get; set; } = null!;
}
