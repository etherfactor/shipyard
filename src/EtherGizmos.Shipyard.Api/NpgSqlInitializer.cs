using System.Runtime.CompilerServices;

namespace EtherGizmos.Shipyard;

public static class NpgSqlInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
}
