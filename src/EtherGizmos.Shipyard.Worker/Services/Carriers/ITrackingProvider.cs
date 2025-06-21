namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

public interface ITrackingProvider
{
    Task<TrackingResult> TrackAsync(string trackingNumber, CancellationToken cancellationToken = default);
}
