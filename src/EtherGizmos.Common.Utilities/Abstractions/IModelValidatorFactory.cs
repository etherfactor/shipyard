namespace EtherGizmos.Common.Utilities.Abstractions;

public interface IModelValidatorFactory
{
    IModelValidator<TModel> GetValidator<TModel>();
}
