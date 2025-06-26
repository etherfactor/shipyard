using EtherGizmos.Shipyard.Notifications.Services;
using System.Net.Mail;
using System.Text;

namespace EtherGizmos.Shipyard.Notifications.Models;

public record PackageOutForDeliveryEvent : NotificationEvent
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

public class PackageOutForDeliveryRenderer : INotificationRenderer<PackageOutForDeliveryEvent>
{
    public Task<MailMessage> RenderAsync(
        PackageOutForDeliveryEvent notification,
        CancellationToken cancellationToken = default)
    {
        var subject = $"Package Out for Delivery: {notification.Contents ?? "Unnamed Package"}";

        var sb = new StringBuilder();
        sb.AppendLine($"Your package is out for delivery!");
        sb.AppendLine();
        sb.AppendLine($"Carrier: {notification.CarrierName}");
        sb.AppendLine($"Tracking Number: {notification.TrackingNumber}");

        if (!string.IsNullOrWhiteSpace(notification.Location))
            sb.AppendLine($"Last Known Location: {notification.Location}");

        if (!string.IsNullOrWhiteSpace(notification.Description))
            sb.AppendLine($"Status Description: {notification.Description}");

        sb.AppendLine($"Time of Update: {notification.OccurredAt:dddd, MMMM d, yyyy h:mm tt}");

        if (notification.EstimatedDeliveryAt is not null)
            sb.AppendLine($"Estimated Delivery: {notification.EstimatedDeliveryAt:dddd, MMMM d, yyyy}");

        sb.AppendLine();
        sb.AppendLine("You can track your package for the latest updates.");

        var message = new MailMessage
        {
            Subject = subject,
            Body = sb.ToString(),
            IsBodyHtml = false
        };

        return Task.FromResult(message);
    }
}
