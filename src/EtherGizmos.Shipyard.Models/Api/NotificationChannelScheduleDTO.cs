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
