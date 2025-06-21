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
        string slug)
    {
        return slug switch
        {
            "usps" => new UspsBrowserTrackingProvider(_serviceProvider),
            _ => throw new NotSupportedException()
        };
    }
}
