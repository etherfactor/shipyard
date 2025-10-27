using AutoMapper;
using EtherGizmos.Shipyard.Api.Enums;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class TrackingUpdateDTO
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public DateTimeOffset OccurredAt { get; set; }

    public StatusTypeDTO StatusType { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }
}

public class TrackingUpdateDTOProfile : Profile
{
    public TrackingUpdateDTOProfile() : base(nameof(TrackingUpdateDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<TrackingUpdate, TrackingUpdateDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        /*  End Audit  */
        toDto.MapMember(dest => dest.OccurredAt, src => src.OccurredAt);
        toDto.MapMember(dest => dest.StatusType, src => src.StatusTypeId);
        toDto.MapMember(dest => dest.Location, src => src.Location);
        toDto.MapMember(dest => dest.Description, src => src.Description);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class TrackingUpdateDTOExamples
{
    public static TrackingUpdateDTO Get { get; } = new()
    {
        Id = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow,
        OccurredAt = DateTimeOffset.UtcNow,
        StatusType = StatusTypeDTO.Delivered,
        Location = "Chicago, IL 60007",
        Description = "Delivery completed; more comments",
    };

    public static TrackingUpdateDTO Post { get; } = Get;

    public static TrackingUpdateDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class TrackingUpdateDTOExampleGet : IExamplesProvider<TrackingUpdateDTO>
{
    public TrackingUpdateDTO GetExamples()
    {
        return TrackingUpdateDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class TrackingUpdateDTOExamplePost : IExamplesProvider<TrackingUpdateDTO>
{
    public TrackingUpdateDTO GetExamples()
    {
        return TrackingUpdateDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class TrackingUpdateDTOExamplePatch : IExamplesProvider<TrackingUpdateDTO>
{
    public TrackingUpdateDTO GetExamples()
    {
        return TrackingUpdateDTOExamples.Patch;
    }
}
