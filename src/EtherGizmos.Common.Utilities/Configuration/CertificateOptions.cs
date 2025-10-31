using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Common.Configuration;

public class CertificateOptions
{
    [Required]
    public CertificateType Type { get; set; }

    #region File
    public FileCertificateOptions? File { get; set; }
    #endregion File

    #region Text
    public TextCertificateOptions? Text { get; set; }
    #endregion Text
}
