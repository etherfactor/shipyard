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
        var provider = slug switch
        {
            //"usps" => new UspsBrowserTrackingProvider(_serviceProvider),
            _ => new RunbookBrowserTrackingProvider(_serviceProvider, slug)
        };

        return new ClassifierTrackingProvider(_serviceProvider, slug, provider);
    }
}
