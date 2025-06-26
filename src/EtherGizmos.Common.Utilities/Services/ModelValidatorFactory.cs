using EtherGizmos.Common.Utilities.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.Common.Utilities.Services;

internal class ModelValidatorFactory : IModelValidatorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ModelValidatorFactory(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IModelValidator<TModel> GetValidator<TModel>()
    {
        var inner = _serviceProvider.GetRequiredService<IEnumerable<IModelValidator<TModel>>>();
        var validator = new AggregateModelValidator<TModel>(inner);

        return validator;
    }

    private class AggregateModelValidator<TModel> : IModelValidator<TModel>
    {
        private readonly IEnumerable<IModelValidator<TModel>> _validators;

        public AggregateModelValidator(
            IEnumerable<IModelValidator<TModel>> validators)
        {
            _validators = validators;
        }

        public async Task ValidateAsync(
            TModel model,
            CancellationToken cancellationToken = default)
        {
            foreach (var validator in _validators)
            {
                await validator.ValidateAsync(model, cancellationToken);
            }
        }
    }
}
