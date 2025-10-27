using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

internal class ClassifierTrackingProvider : ITrackingProvider, IDisposable
{
    private readonly IRegexClassifier _classifier;
    private readonly string _slug;
    private readonly ITrackingProvider _inner;

    private bool _disposed;

    public ClassifierTrackingProvider(
        IServiceProvider serviceProvider,
        string slug,
        ITrackingProvider inner)
    {
        _classifier = serviceProvider.GetRequiredService<IRegexClassifier>();
        _slug = slug;
        _inner = inner;
    }

    public async Task<TrackingResult> TrackAsync(
        string trackingNumber,
        CancellationToken cancellationToken = default)
    {
        var result = await _inner.TrackAsync(trackingNumber, cancellationToken);

        var newStatuses = new List<TrackingResultDetail>();
        foreach (var status in result.Details)
        {
            if (status.Description is not null)
            {
                var statusTypeId = await _classifier.ClassifyStatusAsync(_slug, status.Description, cancellationToken: cancellationToken);
                newStatuses.Add(status with
                {
                    StatusTypeId = statusTypeId,
                });
            }
            else
            {
                newStatuses.Add(status);
            }
        }

        return result with { Details = newStatuses };
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _inner.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
