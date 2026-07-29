using AutoMapper;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class NotificationScheduleDTO
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DynamicBagDTO ConfigSchema { get; set; } = new();
}

public class NotificationScheduleDTOProfile : Profile
{
    public NotificationScheduleDTOProfile() : base(nameof(NotificationScheduleDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<NotificationSchedule, NotificationScheduleDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        /*  End Audit  */
        toDto.MapMember(dest => dest.Name, src => src.Name);
        toDto.MapMember(dest => dest.ConfigSchema, src => new DynamicBagDTO() { Data = src.ConfigSchema });
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class NotificationScheduleDTOExamples
{
    public static NotificationScheduleDTO Get { get; } = new()
    {
        Id = "immediate",
        Name = "Immediate",
        ConfigSchema = new(),
    };

    public static NotificationScheduleDTO Post { get; } = Get;

    public static NotificationScheduleDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class NotificationScheduleDTOExampleGet : IExamplesProvider<NotificationScheduleDTO>
{
    public NotificationScheduleDTO GetExamples()
    {
        return NotificationScheduleDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class NotificationScheduleDTOExamplePost : IExamplesProvider<NotificationScheduleDTO>
{
    public NotificationScheduleDTO GetExamples()
    {
        return NotificationScheduleDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class NotificationScheduleDTOExamplePatch : IExamplesProvider<NotificationScheduleDTO>
{
    public NotificationScheduleDTO GetExamples()
    {
        return NotificationScheduleDTOExamples.Patch;
    }
}
