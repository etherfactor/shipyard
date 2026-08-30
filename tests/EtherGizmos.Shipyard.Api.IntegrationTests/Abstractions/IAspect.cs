namespace EtherGizmos.Shipyard.Abstractions;

public interface IAspect<TEntity, TId>
    where TEntity : class, new()
{
    string Name { get; }

    IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification);
}
