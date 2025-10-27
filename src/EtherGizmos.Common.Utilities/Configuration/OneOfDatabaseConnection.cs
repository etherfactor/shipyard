using OneOf;

namespace EtherGizmos.Common.Utilities.Configuration;

[GenerateOneOf]
public partial class OneOfDatabaseConnection : OneOfBase<DatabaseConnectionOptions, PostgreSqlOptions> { }
