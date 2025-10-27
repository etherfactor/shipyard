namespace EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;

public interface IODataResourceSpec<TEntity, TId>
    where TEntity : class, new()
{
    string BaseRoute { get; }

    IReadOnlySet<ODataCapability> Capabilities { get; }

    Func<TEntity, TId> Identity { get; }

    Func<TId, string> Path { get; }

    IRecordSource<TEntity, TId> Records { get; }

    HttpContent Create();

    HttpContent Update(TEntity entity);
}
