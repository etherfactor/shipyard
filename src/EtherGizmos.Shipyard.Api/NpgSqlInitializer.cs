using System.Runtime.CompilerServices;

namespace EtherGizmos.Shipyard.Api;

public static class NpgSqlInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
}
