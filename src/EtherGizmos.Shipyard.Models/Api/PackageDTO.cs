using AutoMapper;
using EtherGizmos.Shipyard.Api.Enums;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class PackageDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    [Required]
    public int? CarrierId { get; set; }

    public CarrierDTO? Carrier { get; set; }

    [Required]
    public string TrackingNumber { get; set; } = null!;

    public string? Contents { get; set; }

    public DateTimeOffset? EstimatedDeliveryAt { get; set; }

    public DateTimeOffset LastPollAt { get; set; }

    public DateTimeOffset NextPollAt { get; set; }

    public StatusTypeDTO LastStatusType { get; set; }

    public bool IsDelivered { get; set; }

    public List<TrackingUpdateDTO> TrackingUpdates { get; set; } = [];
}

public class PackageDTOProfile : Profile
{
    public PackageDTOProfile() : base(nameof(PackageDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<Package, PackageDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        /*  End Audit  */
        toDto.MapMember(dest => dest.CarrierId, src => src.CarrierId);
        toDto.MapMember(dest => dest.Carrier, src => src.Carrier, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.TrackingNumber, src => src.TrackingNumber);
        toDto.MapMember(dest => dest.Contents, src => src.Contents);
        toDto.MapMember(dest => dest.EstimatedDeliveryAt, src => src.EstimatedDeliveryAt);
        toDto.MapMember(dest => dest.LastPollAt, src => src.LastPollAt);
        toDto.MapMember(dest => dest.NextPollAt, src => src.NextPollAt);
        toDto.MapMember(dest => dest.LastStatusType, src => src.LastStatusTypeId);
        toDto.MapMember(dest => dest.IsDelivered, src => src.IsDelivered);
        toDto.MapMember(dest => dest.TrackingUpdates, src => src.TrackingUpdates, opt => opt.ExplicitExpansion());

        var fromDto = mapper.CreateMap<PackageDTO, Package>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.CarrierId, src => src.CarrierId);
        fromDto.MapMember(dest => dest.TrackingNumber, src => src.TrackingNumber);
        fromDto.MapMember(dest => dest.EstimatedDeliveryAt, src => src.EstimatedDeliveryAt);
        fromDto.MapMember(dest => dest.Contents, src => src.Contents);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class PackageDTOExamples
{
    public static PackageDTO Get { get; } = new()
    {
        Id = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow,
        CarrierId = 1,
        TrackingNumber = "123456789",
        Contents = "Some items",
        LastPollAt = DateTimeOffset.UtcNow,
        NextPollAt = DateTimeOffset.UtcNow,
        LastStatusType = StatusTypeDTO.Delivered,
        IsDelivered = true,
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
