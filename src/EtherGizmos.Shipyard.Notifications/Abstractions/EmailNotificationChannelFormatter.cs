using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Abstractions;

public abstract class EmailNotificationChannelFormatter<TSchedule, TModel>
    : NotificationChannelFormatter<TSchedule, EmailChannel, EmailEnvelope, TModel>
    where TSchedule : NotificationScheduleRef
    where TModel : class;
