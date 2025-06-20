using EtherGizmos.Shipyard.Models.Api.Enums;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Models.Api;

public class PackageDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public int CarrierId { get; set; }

    public CarrierDTO Carrier { get; set; } = null!;

    public string TrackingNumber { get; set; } = null!;

    public string? Contents { get; set; }

    public DateTimeOffset LastPollAt { get; set; }

    public DateTimeOffset NextPollAt { get; set; }

    public StatusTypeDTO StatusType { get; set; }

    public bool IsDelivered { get; set; }
}

[ExcludeFromCodeCoverage]
public static class PackageDTOExamples
{
    public static PackageDTO Get { get; } = new()
    {

    };

    public static PackageDTO Post { get; } = Get;

    public static PackageDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class PackageDTOExampleGet : IExamplesProvider<PackageDTO>
{
    public PackageDTO GetExamples()
    {
        return PackageDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class PackageDTOExamplePost : IExamplesProvider<PackageDTO>
{
    public PackageDTO GetExamples()
    {
        return PackageDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class PackageDTOExamplePatch : IExamplesProvider<PackageDTO>
{
    public PackageDTO GetExamples()
    {
        return PackageDTOExamples.Patch;
    }
}
