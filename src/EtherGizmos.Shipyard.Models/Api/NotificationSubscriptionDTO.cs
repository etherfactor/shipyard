using AutoMapper;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class NotificationSubscriptionDTO
{
    public long Id { get; set; }

    public Guid UserId { get; set; }

    [Required]
    public string EventType { get; set; } = null!;

    [Required]
    public string ChannelKey { get; set; } = null!;

    [Required]
    public DynamicBagDTO ChannelConfig { get; set; } = new();

    [Required]
    public string ScheduleType { get; set; } = null!;

    [Required]
    public DynamicBagDTO ScheduleConfig { get; set; } = new();

    public bool IsActive { get; set; }

    public DateTimeOffset? LastNotificationAt { get; set; }

    public DateTimeOffset? NextNotificationAt { get; set; }
}

public class NotificationSubscriptionDTOProfile : Profile
{
    public NotificationSubscriptionDTOProfile() : base(nameof(NotificationSubscriptionDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<NotificationSubscription, NotificationSubscriptionDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        toDto.MapMember(dest => dest.UserId, src => Guid.Parse(src.UserId));
        toDto.MapMember(dest => dest.EventType, src => src.EventType);
        toDto.MapMember(dest => dest.ChannelKey, src => src.ChannelKey);
        toDto.MapMember(dest => dest.ChannelConfig, src => new DynamicBagDTO() { DataRaw = src.ChannelConfigRaw });
        toDto.MapMember(dest => dest.ScheduleType, src => src.ScheduleType);
        toDto.MapMember(dest => dest.ScheduleConfig, src => new DynamicBagDTO() { DataRaw = src.ScheduleConfigRaw });
        toDto.MapMember(dest => dest.IsActive, src => src.IsEnabled);
        toDto.MapMember(dest => dest.LastNotificationAt, src => src.LastNotificationAt);
        toDto.MapMember(dest => dest.NextNotificationAt, src => src.NextNotificationAt);

        var fromDto = mapper.CreateMap<NotificationSubscriptionDTO, NotificationSubscription>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.UserId, src => src.UserId.ToString());
        fromDto.MapMember(dest => dest.EventType, src => src.EventType);
        fromDto.MapMember(dest => dest.ChannelKey, src => src.ChannelKey);
        fromDto.MapMember(dest => dest.ChannelConfigRaw, src => src.ChannelConfig.DataRaw);
        fromDto.MapMember(dest => dest.ScheduleType, src => src.ScheduleType);
        fromDto.MapMember(dest => dest.ScheduleConfigRaw, src => src.ScheduleConfig.DataRaw);
        fromDto.MapMember(dest => dest.IsEnabled, src => src.IsActive);
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class NotificationSubscriptionDTOExamples
{
    public static NotificationSubscriptionDTO Get { get; } = new()
    {
        Id = 1,
        UserId = Guid.NewGuid(),
        EventType = "my.event",
        ChannelKey = "email",
        ChannelConfig = DynamicBagDTOExamples.Get,
        ScheduleType = "immediate",
        ScheduleConfig = DynamicBagDTOExamples.Get,
        IsActive = true,
        LastNotificationAt = DateTimeOffset.UtcNow,
        NextNotificationAt = DateTimeOffset.UtcNow,
    };

    public static NotificationSubscriptionDTO Post { get; } = Get;

    public static NotificationSubscriptionDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class NotificationSubscriptionDTOExampleGet : IExamplesProvider<NotificationSubscriptionDTO>
{
    public NotificationSubscriptionDTO GetExamples()
    {
        return NotificationSubscriptionDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class NotificationSubscriptionDTOExamplePost : IExamplesProvider<NotificationSubscriptionDTO>
{
    public NotificationSubscriptionDTO GetExamples()
    {
        return NotificationSubscriptionDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class NotificationSubscriptionDTOExamplePatch : IExamplesProvider<NotificationSubscriptionDTO>
{
    public NotificationSubscriptionDTO GetExamples()
    {
        return NotificationSubscriptionDTOExamples.Patch;
    }
}
