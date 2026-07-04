using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public class PackageFailedAttemptDigestWebhookFormatter
    : WebhookNotificationChannelFormatter<DigestSchedule, Digest<PackageFailedAttemptEvent>>;
