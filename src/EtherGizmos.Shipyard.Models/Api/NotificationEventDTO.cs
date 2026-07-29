using AutoMapper;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class NotificationEventDTO
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public List<NotificationChannelScheduleDTO> Supports { get; set; } = [];
}

public class NotificationEventDTOProfile : Profile
{
    public NotificationEventDTOProfile() : base(nameof(NotificationEventDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<NotificationEvent, NotificationEventDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        /*  End Audit  */
        toDto.MapMember(dest => dest.Name, src => src.Name);
        toDto.MapMember(dest => dest.Supports, src => src.Supports);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class NotificationEventDTOExamples
{
    public static NotificationEventDTO Get { get; } = new()
    {
        Id = "package.delivered",
        Name = "Package Delivered",
        Supports = [NotificationChannelScheduleDTOExamples.Get],
    };

    public static NotificationEventDTO Post { get; } = Get;

    public static NotificationEventDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class NotificationEventDTOExampleGet : IExamplesProvider<NotificationEventDTO>
{
    public NotificationEventDTO GetExamples()
    {
        return NotificationEventDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class NotificationEventDTOExamplePost : IExamplesProvider<NotificationEventDTO>
{
    public NotificationEventDTO GetExamples()
    {
        return NotificationEventDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class NotificationEventDTOExamplePatch : IExamplesProvider<NotificationEventDTO>
{
    public NotificationEventDTO GetExamples()
    {
        return NotificationEventDTOExamples.Patch;
    }
}
