using AutoMapper;
using EtherGizmos.Shipyard.Models.Api.Enums;
using EtherGizmos.Shipyard.Models.Database;
using EtherGizmos.Shipyard.Models.Extensions;
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

    public StatusTypeDTO LastStatusType { get; set; }

    public bool IsDelivered { get; set; }
}

public class PackageDTOProfile : Profile
{
    public PackageDTOProfile() : base(nameof(PackageDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<Package, PackageDTO>();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        /*  End Audit  */
        toDto.MapMember(dest => dest.CarrierId, src => src.CarrierId);
        toDto.MapMember(dest => dest.Carrier, src => src.Carrier, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.TrackingNumber, src => src.TrackingNumber);
        toDto.MapMember(dest => dest.Contents, src => src.Contents);
        toDto.MapMember(dest => dest.LastPollAt, src => src.LastPollAt);
        toDto.MapMember(dest => dest.NextPollAt, src => src.NextPollAt);
        toDto.MapMember(dest => dest.LastStatusType, src => src.LastStatusTypeId);
        toDto.MapMember(dest => dest.IsDelivered, src => src.IsDelivered);
    })
    { }
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
