using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Api;

public class NotificationChannelDTO
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DynamicBagDTO ConfigSchema { get; set; } = new();
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
