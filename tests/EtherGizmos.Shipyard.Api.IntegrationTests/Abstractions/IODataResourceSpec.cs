namespace EtherGizmos.Shipyard.Abstractions;

public interface IODataResourceSpec<TEntity, TId>
    where TEntity : class, new()
{
    string BaseRoute { get; }

    IReadOnlySet<ResourceFunctionality> Capabilities { get; }

    Func<TEntity, TId> Identity { get; }

    Func<TId, string> Path { get; }

    IRecordSource<TEntity, TId> Records { get; }

    HttpContent Create();

    HttpContent Update(TEntity entity);
}
