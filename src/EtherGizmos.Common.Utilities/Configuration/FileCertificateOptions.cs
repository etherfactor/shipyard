using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Common.Configuration;

public class FileCertificateOptions : CertificateReferenceOptions
{
    [Required]
    public string Path { get; set; } = null!;

    public string? Password { get; set; }

    public bool AutoGenerate { get; set; } = false;
}
