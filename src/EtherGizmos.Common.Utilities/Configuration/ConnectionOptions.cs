using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Common.Utilities.Configuration;

public class ConnectionOptions
{
    [Required]
    public ConnectionType Type { get; set; }

    #region Database
    public PostgreSqlOptions? PostgreSql { get; set; }
    #endregion Database

    #region Email
    public SmtpOptions? Smtp { get; set; }
    #endregion Email
}
