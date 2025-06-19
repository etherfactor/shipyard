namespace EtherGizmos.Shipyard.Utilities.Abstractions;

public interface IModelValidator<TModel>
{
    public Task ValidateAsync(TModel model);
}
