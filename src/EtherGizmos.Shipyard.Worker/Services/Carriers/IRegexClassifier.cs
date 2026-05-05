namespace EtherGizmos.Shipyard.Services.Carriers;

public interface IRegexClassifier
{
    Task<int> ClassifyStatusAsync(int carrierId, string description, CancellationToken cancellationToken = default);
}
