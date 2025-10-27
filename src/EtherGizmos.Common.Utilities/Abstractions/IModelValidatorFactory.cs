namespace EtherGizmos.Shipyard.Utilities.Abstractions;

public interface IModelValidatorFactory
{
    IModelValidator<TModel> GetValidator<TModel>();
}
