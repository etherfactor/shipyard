using Asp.Versioning;

namespace EtherGizmos.Shipyard.Models;

public static class ApiVersions
{
    public static ApiVersion V0_1 { get; } = new(0, 1);
}
