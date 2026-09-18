namespace EtherGizmos.Shipyard.Services.Carriers;

public interface ITrackingProvider : IDisposable
{
    Task<TrackingResult> TrackAsync(string trackingNumber, CancellationToken cancellationToken = default);
}
