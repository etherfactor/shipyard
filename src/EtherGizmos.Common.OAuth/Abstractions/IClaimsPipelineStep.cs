namespace EtherGizmos.Common.Abstractions;

public interface IClaimsPipelineStep<TContext>
    where TContext : IClaimsContext
{
    int Order { get; }

    Task<TContext> ExecuteAsync(
        TContext context,
        CancellationToken cancellationToken = default);
}
