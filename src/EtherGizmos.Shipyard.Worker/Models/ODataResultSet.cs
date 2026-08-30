namespace EtherGizmos.Shipyard.Models;

public class ODataResultSet<TEntity>
    where TEntity : class
{
    public IReadOnlyList<TEntity> Value { get; set; } = [];
}
