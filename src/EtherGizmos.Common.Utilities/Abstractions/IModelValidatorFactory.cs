namespace EtherGizmos.Common.Abstractions;

public interface IModelValidatorFactory
{
    IModelValidator<TModel> GetValidator<TModel>();
}
