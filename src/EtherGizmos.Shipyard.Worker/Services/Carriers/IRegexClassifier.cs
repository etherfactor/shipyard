namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

public interface IRegexClassifier
{
    Task<int> ClassifyStatusAsync(int carrierId, string description, CancellationToken cancellationToken = default);
}
