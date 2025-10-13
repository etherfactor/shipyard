namespace EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;

public interface IAspect<TEntity, TId>
    where TEntity : class, new()
{
    string Name { get; }

    IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification,
        FixtureContext context);
}
