using AutoMapper;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class NotificationChannelDTO
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DynamicBagDTO ConfigSchema { get; set; } = new();
}

public class NotificationChannelDTOProfile : Profile
{
    public NotificationChannelDTOProfile() : base(nameof(NotificationChannelDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<NotificationChannel, NotificationChannelDTO>();
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
public static class NotificationChannelDTOExamples
{
    public static NotificationChannelDTO Get { get; } = new()
    {
        Id = "email",
        Name = "Email",
        ConfigSchema = new(),
    };

    public static NotificationChannelDTO Post { get; } = Get;

    public static NotificationChannelDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class NotificationChannelDTOExampleGet : IExamplesProvider<NotificationChannelDTO>
{
    public NotificationChannelDTO GetExamples()
    {
        return NotificationChannelDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class NotificationChannelDTOExamplePost : IExamplesProvider<NotificationChannelDTO>
{
    public NotificationChannelDTO GetExamples()
    {
        return NotificationChannelDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class NotificationChannelDTOExamplePatch : IExamplesProvider<NotificationChannelDTO>
{
    public NotificationChannelDTO GetExamples()
    {
        return NotificationChannelDTOExamples.Patch;
    }
}
