using OneOf;

namespace EtherGizmos.Common.Utilities.Configuration;

[GenerateOneOf]
public partial class OneOfEmailConnection : OneOfBase<EmailConnectionOptions, SmtpOptions>
{
}
