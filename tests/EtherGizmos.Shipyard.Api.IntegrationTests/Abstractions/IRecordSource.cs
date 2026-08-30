namespace EtherGizmos.Shipyard.Abstractions;

public interface IRecordSource<TEntity, TId>
    where TEntity : class, new()
{
    Task<(TEntity Entity, TId Id)> AcquireAsync(
        FixtureContext context,
        AcquirePurpose purpose,
        Guid? createdByUserId = null);
}

public enum AcquirePurpose
{
    ForRead = 1,
    ForUpdate = 2,
    ForDelete = 3,
}
