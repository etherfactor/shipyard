namespace EtherGizmos.Common.Abstractions;

public interface IModelValidator<TModel>
{
    Task ValidateAsync(TModel model, CancellationToken cancellationToken = default);
}
