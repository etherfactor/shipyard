namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

public interface IRegexClassifier
{
    Task<int> ClassifyStatusAsync(string slug, string description, CancellationToken cancellationToken = default);
}
