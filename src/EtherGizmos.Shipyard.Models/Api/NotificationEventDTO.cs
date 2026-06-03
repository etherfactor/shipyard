using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class NotificationEventDTO
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public List<NotificationChannelScheduleDTO> Supports { get; set; } = [];
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
