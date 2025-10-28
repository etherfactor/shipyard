namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

internal class TrackingProviderFactory : ITrackingProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TrackingProviderFactory(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITrackingProvider CreateProvider(
        int carrierId,
        int executionId)
    {
        var provider = new RunbookBrowserTrackingProvider(_serviceProvider, carrierId, executionId);

        return new ClassifierTrackingProvider(_serviceProvider, carrierId, provider);
    }
}
