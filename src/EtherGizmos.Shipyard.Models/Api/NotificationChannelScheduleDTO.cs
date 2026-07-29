using AutoMapper;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class NotificationChannelScheduleDTO
{
    public string NotificationChannelId { get; set; } = null!;

    public NotificationChannelDTO? NotificationChannel { get; set; }

    public string NotificationScheduleId { get; set; } = null!;

    public NotificationScheduleDTO? NotificationSchedule { get; set; }
}

public class NotificationChannelScheduleDTOProfile : Profile
{
    public NotificationChannelScheduleDTOProfile() : base(nameof(NotificationChannelScheduleDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<NotificationChannelSchedule, NotificationChannelScheduleDTO>();
        toDto.IgnoreAllMembers();
        /* Begin Audit */
        /*  End Audit  */
        toDto.MapMember(dest => dest.NotificationChannelId, src => src.ChannelId);
        toDto.MapMember(dest => dest.NotificationChannel, src => src.Channel, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.NotificationScheduleId, src => src.ScheduleId);
        toDto.MapMember(dest => dest.NotificationSchedule, src => src.Schedule, opt => opt.ExplicitExpansion());
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class NotificationChannelScheduleDTOExamples
{
    public static NotificationChannelScheduleDTO Get { get; } = new()
    {
        NotificationChannelId = "email",
        NotificationScheduleId = "immediate",
    };

    public static NotificationChannelScheduleDTO Post { get; } = Get;

    public static NotificationChannelScheduleDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class NotificationChannelScheduleDTOExampleGet : IExamplesProvider<NotificationChannelScheduleDTO>
{
    public NotificationChannelScheduleDTO GetExamples()
    {
        return NotificationChannelScheduleDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class NotificationChannelScheduleDTOExamplePost : IExamplesProvider<NotificationChannelScheduleDTO>
{
    public NotificationChannelScheduleDTO GetExamples()
    {
        return NotificationChannelScheduleDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class NotificationChannelScheduleDTOExamplePatch : IExamplesProvider<NotificationChannelScheduleDTO>
{
    public NotificationChannelScheduleDTO GetExamples()
    {
        return NotificationChannelScheduleDTOExamples.Patch;
    }
}
