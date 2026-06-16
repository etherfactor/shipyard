using AutoMapper;
using EtherGizmos.Shipyard.Database;
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
    public string NotificationEventId { get; set; } = null!;

    public NotificationEventDTO? NotificationEvent { get; set; }

    [Required]
    public string NotificationChannelId { get; set; } = null!;

    public NotificationChannelDTO? NotificationChannel { get; set; }

    [Required]
    public DynamicBagDTO NotificationChannelConfig { get; set; } = new();

    [Required]
    public string NotificationScheduleId { get; set; } = null!;

    public NotificationScheduleDTO? NotificationSchedule { get; set; }

    [Required]
    public DynamicBagDTO NotificationScheduleConfig { get; set; } = new();

    public bool IsActive { get; set; }

    public DateTimeOffset? LastNotificationAt { get; set; }

    public DateTimeOffset? NextNotificationAt { get; set; }
}

public class NotificationSubscriptionDTOProfile : Profile
{
    public NotificationSubscriptionDTOProfile() : base(nameof(NotificationSubscriptionDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<AppNotificationSubscription, NotificationSubscriptionDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        toDto.MapMember(dest => dest.UserId, src => Guid.Parse(src.UserId));
        toDto.MapMember(dest => dest.NotificationEventId, src => src.EventType);
        toDto.MapMember(dest => dest.NotificationEvent, src => src.Event, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.NotificationChannelId, src => src.ChannelKey);
        toDto.MapMember(dest => dest.NotificationChannel, src => src.Channel, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.NotificationChannelConfig, src => new DynamicBagDTO() { DataRaw = src.ChannelConfigRaw });
        toDto.MapMember(dest => dest.NotificationScheduleId, src => src.ScheduleType);
        toDto.MapMember(dest => dest.NotificationSchedule, src => src.Schedule, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.NotificationScheduleConfig, src => new DynamicBagDTO() { DataRaw = src.ScheduleConfigRaw });
        toDto.MapMember(dest => dest.IsActive, src => src.IsEnabled);
        toDto.MapMember(dest => dest.LastNotificationAt, src => src.LastNotificationAt);
        toDto.MapMember(dest => dest.NextNotificationAt, src => src.NextNotificationAt);

        var fromDto = mapper.CreateMap<NotificationSubscriptionDTO, AppNotificationSubscription>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.UserId, src => src.UserId.ToString());
        fromDto.MapMember(dest => dest.EventType, src => src.NotificationEventId);
        fromDto.MapMember(dest => dest.ChannelKey, src => src.NotificationChannelId);
        fromDto.MapMember(dest => dest.ChannelConfigRaw, src => src.NotificationChannelConfig.DataRaw);
        fromDto.MapMember(dest => dest.ScheduleType, src => src.NotificationScheduleId);
        fromDto.MapMember(dest => dest.ScheduleConfigRaw, src => src.NotificationScheduleConfig.DataRaw);
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
        NotificationEventId = "my.event",
        NotificationChannelId = "email",
        NotificationChannelConfig = DynamicBagDTOExamples.Get,
        NotificationScheduleId = "immediate",
        NotificationScheduleConfig = DynamicBagDTOExamples.Get,
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
