using OneOf;

namespace EtherGizmos.Common.Configuration;

[GenerateOneOf]
public partial class OneOfCertificateReference : OneOfBase<CertificateReferenceOptions, FileCertificateOptions, TextCertificateOptions>;
