using AutoMapper;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class NotificationDTO
{
    public long Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? SentAt { get; set; }

    public long NotificationSubscriptionId { get; set; }

    public NotificationSubscriptionDTO NotificationSubscription { get; set; } = null!;

    public DynamicBagDTO Payload { get; set; } = new();
}

public class NotificationDTOProfile : Profile
{
    public NotificationDTOProfile() : base(nameof(NotificationDTOProfile), mapper =>
    {
        var toDto = mapper.CreateMap<AppNotification, NotificationDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.SentAt, src => src.SentAt);
        toDto.MapMember(dest => dest.NotificationSubscriptionId, src => src.NotificationSubscriptionId);
        toDto.MapMember(dest => dest.NotificationSubscription, src => src.NotificationSubscription, opt => opt.ExplicitExpansion());
        toDto.MapMember(dest => dest.Payload, src => new DynamicBagDTO() { DataRaw = src.Payload });
    })
    { }
}

[ExcludeFromCodeCoverage]
public static class NotificationDTOExamples
{
    public static NotificationDTO Get { get; } = new()
    {
        Id = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        SentAt = DateTimeOffset.UtcNow,
        NotificationSubscriptionId = 1,
        NotificationSubscription = null!,
        Payload = new()
        {
            Data = new Dictionary<string, object?>()
            {
                ["key"] = "Value",
            },
        },
    };

    public static NotificationDTO Post { get; } = Get;

    public static NotificationDTO Patch { get; } = Post;
}

[ExcludeFromCodeCoverage]
public class NotificationDTOExampleGet : IExamplesProvider<NotificationDTO>
{
    public NotificationDTO GetExamples()
    {
        return NotificationDTOExamples.Get;
    }
}

[ExcludeFromCodeCoverage]
public class NotificationDTOExamplePost : IExamplesProvider<NotificationDTO>
{
    public NotificationDTO GetExamples()
    {
        return NotificationDTOExamples.Post;
    }
}

[ExcludeFromCodeCoverage]
public class NotificationDTOExamplePatch : IExamplesProvider<NotificationDTO>
{
    public NotificationDTO GetExamples()
    {
        return NotificationDTOExamples.Patch;
    }
}
