using EtherGizmos.Common.Abstractions;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public class PackageOutForDeliveryWebhookFormatter
    : WebhookNotificationChannelFormatter<ImmediateSchedule, PackageOutForDeliveryEvent>;
