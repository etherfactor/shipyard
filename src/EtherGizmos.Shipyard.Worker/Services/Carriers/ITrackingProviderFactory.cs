namespace EtherGizmos.Shipyard.Services.Carriers;

public interface ITrackingProviderFactory
{
    ITrackingProvider CreateProvider(int carrierId, int executionId);
}
