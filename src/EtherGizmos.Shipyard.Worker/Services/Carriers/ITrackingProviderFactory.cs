namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

public interface ITrackingProviderFactory
{
    ITrackingProvider CreateProvider(string slug);
}
