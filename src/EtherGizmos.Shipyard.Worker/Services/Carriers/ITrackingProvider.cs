namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

public interface ITrackingProvider : IDisposable
{
    Task<TrackingResult> TrackAsync(string trackingNumber, CancellationToken cancellationToken = default);
}
