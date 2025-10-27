using EtherGizmos.Shipyard.Services;
using MimeKit;
using System.Text;

namespace EtherGizmos.Shipyard.Models;

public record PackageDeliveredEvent : NotificationEvent
{
    public required int PackageId { get; init; }

    public required int CarrierId { get; init; }

    public required string CarrierName { get; init; }

    public required string TrackingNumber { get; init; }

    public required string? Contents { get; init; }

    public required DateTimeOffset OccurredAt { get; init; }

    public required string? Location { get; init; }

    public required string? Description { get; init; }

    public required DateTimeOffset? EstimatedDeliveryAt { get; init; }
}

public class PackageDeliveredRenderer : INotificationRenderer<PackageDeliveredEvent>
{
    public Task<MimeMessage> RenderAsync(
        PackageDeliveredEvent notification,
        CancellationToken cancellationToken = default)
    {
        var subject = $"Package Delivered: {notification.Contents ?? "Unnamed Package"}";

        var sb = new StringBuilder();
        sb.AppendLine($"Your package has been delivered!");
        sb.AppendLine();
        sb.AppendLine($"Carrier: {notification.CarrierName}");
        sb.AppendLine($"Tracking Number: {notification.TrackingNumber}");

        if (!string.IsNullOrWhiteSpace(notification.Location))
            sb.AppendLine($"Delivered To: {notification.Location}");

        if (!string.IsNullOrWhiteSpace(notification.Description))
            sb.AppendLine($"Delivery Notes: {notification.Description}");

        sb.AppendLine($"Delivery Time: {notification.OccurredAt:dddd, MMMM d, yyyy h:mm tt}");

        sb.AppendLine();
        sb.AppendLine("If you can't locate your package, please check around your delivery location or contact the carrier for help.");

        var message = new MimeMessage
        {
            Subject = subject,
            Body = new TextPart("plain")
            {
                Text = sb.ToString(),
            },
        };

        return Task.FromResult(message);
    }

    public async Task<object> RenderAsync(NotificationEvent notification, CancellationToken cancellationToken = default)
    {
        return await RenderAsync((PackageDeliveredEvent)notification, cancellationToken);
    }
}
