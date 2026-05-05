using AutoMapper;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Linq.Expressions;

namespace EtherGizmos.Shipyard.Controllers;

public abstract class AutoODataController : ODataController
{
    private readonly IServiceProvider _serviceProvider;

    public AutoODataController(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected async Task<TEntity> LoadRecordAsync<TEntity, TDto, TKey>(
        IUnitOfWork uow,
        IEnumerable<KeyMapping<TEntity, TDto, TKey>> keys,
        string target = ErrorConstants.RequestTarget.Uri,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TDto : class, new()
    {
        var repository = uow.Repository<TEntity>();

        //Combine individual keys into a single condition
        var condition = keys
            .Select(e => (e.DbKey, e.Value))
            .Select(e =>
                Expression.Lambda<Func<TEntity, bool>>(
                    Expression.Equal(
                        e.DbKey.Body,
                        Expression.Constant(e.Value)),
                    [.. e.DbKey.Parameters]))
            .Aggregate((left, right) =>
            {
                var parameters = left.Parameters;

                var leftExp = left;
                var rightExp = right;

                return Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(leftExp.Body, rightExp.Body), [.. parameters]);
            });

        var record = await repository.Data.SingleOrDefaultAsync(condition, cancellationToken: cancellationToken);

        //Record was not found, so return 404 Not Found
        if (record is null)
        {
            var error = new Error.Reference.EntityNotFoundReferenceError<TDto>(target);
            foreach (var key in keys.Select(e => (e.DtoKey, e.Value)))
            {
                var selector = Expression.Lambda<Func<TDto, object?>>(Expression.Convert(key.DtoKey.Body, typeof(object)), [.. key.DtoKey.Parameters]);
                error.AddDetail((selector, key.Value));
            }

            error.Return();
        }

        return record;
    }

    protected internal TKey ParseRelatedKey<TEntity, TKey>(
        Uri link,
        string target = ErrorConstants.RequestTarget.Uri,
        int index = 0)
    {
        TKey result = default!;

        var model = Request
            .GetRouteServices()
            .GetRequiredService<IEdmModel>();

        var serviceRoot = Request
            .CreateODataLink();

        var uriParser = new ODataUriParser(model, new Uri(serviceRoot), link);

        var odataPath = uriParser.ParsePath();
        var keySegment = odataPath
            .OfType<KeySegment>()
            .LastOrDefault();

        var maybeKey = keySegment?.Keys?.ElementAt(index).Value;
        if (maybeKey is not TKey parsedKey)
        {
            new Error.Reference.InvalidKeyTypeReferenceError<TEntity>(target)
                .AddDetail((typeof(TKey), maybeKey!))
                .Return();
        }
        else
        {
            result = parsedKey;
        }

        return result;
    }

    protected IKeylessRequestBuilder<TEntity, TDto> ForSet<TEntity, TDto>()
        where TEntity : class, IEntity
        where TDto : class, new()
    {
        return new EntityRequestBuilder<TEntity, TDto, object>(this);
    }

    protected IKeyedRequestBuilder<TEntity, TDto> ForItem<TEntity, TDto, TKey>(
        params KeyMapping<TEntity, TDto, TKey>[] keys)
        where TEntity : class, IEntity
        where TDto : class, new()
    {
        return new EntityRequestBuilder<TEntity, TDto, TKey>(this, keys);
    }

    protected interface IKeylessRequestBuilder<TEntity, TDto>
        where TEntity : class, IEntity
        where TDto : class, new()
    {
        Task<IActionResult> CreateAsync(TDto create, ODataQueryOptions<TDto> queryOptions, CancellationToken cancellationToken = default);

        IKeylessRequestBuilder<TEntity, TDto> OnCreating(Func<TEntity, TDto, Task> beforeSave);

        Task<IActionResult> SearchAsync(ODataQueryOptions<TDto> queryOptions, CancellationToken cancellationToken = default);
    }

    protected interface IKeyedRequestBuilder<TEntity, TDto>
        where TEntity : class, IEntity
        where TDto : class, new()
    {
        Task<IActionResult> DeleteAsync(CancellationToken cancellationToken = default);

        Task<IActionResult> GetAsync(ODataQueryOptions<TDto> queryOptions, CancellationToken cancellationToken = default);

        IKeyedRequestBuilder<TEntity, TDto> OnUpdating(Func<TEntity, TDto, Task> beforeSave);

        Task<IActionResult> PatchAsync(Delta<TDto> patch, ODataQueryOptions<TDto> queryOptions, CancellationToken cancellationToken = default);

        IReferenceRequestBuilder<TEntity, TDto, TFEntity, TFDto> ForReference<TFEntity, TFDto, TFKey>(Func<TEntity, ICollection<TFEntity>> findCollection, params KeyMapping<TFEntity, TFDto, TFKey>[] fkeys)
            where TFEntity : class, IEntity
            where TFDto : class, new();
    }

    protected class KeyMapping<TEntity, TDto, TKey>
    {
        public TKey Value { get; }

        public Expression<Func<TEntity, TKey>> DbKey { get; }

        public Expression<Func<TDto, TKey>> DtoKey { get; }

        private KeyMapping(
            TKey key,
            Expression<Func<TEntity, TKey>> dbKey,
            Expression<Func<TDto, TKey>> dtoKey)
        {
            Value = key;
            DbKey = dbKey;
            DtoKey = dtoKey;
        }

        public static KeyMapping<TEntity, TDto, TKey> Create(
            TKey key,
            Expression<Func<TEntity, TKey>> dbKey,
            Expression<Func<TDto, TKey>> dtoKey)
            => new(key, dbKey, dtoKey);
    }

    private class EntityRequestBuilder<TEntity, TDto, TKey> :
        IKeylessRequestBuilder<TEntity, TDto>,
        IKeyedRequestBuilder<TEntity, TDto>
        where TEntity : class, IEntity
        where TDto : class, new()
    {
        private readonly AutoODataController _controller;
        private readonly IMapper _mapper;
        private readonly IModelValidatorFactory _modelValidatorFactory;
        private readonly IUnitOfWorkFactory _uowFactory;

        private readonly List<Func<TEntity, TDto, Task>> _onCreating = [];
        private readonly List<Func<TEntity, TDto, Task>> _onUpdating = [];

        public List<KeyMapping<TEntity, TDto, TKey>> Keys { get; }

        public EntityRequestBuilder(
            AutoODataController controller,
            params KeyMapping<TEntity, TDto, TKey>[] keys)
        {
            _controller = controller;
            _mapper = controller._serviceProvider.GetRequiredService<IMapper>();
            _modelValidatorFactory = controller._serviceProvider.GetRequiredService<IModelValidatorFactory>();
            _uowFactory = controller._serviceProvider.GetRequiredService<IUnitOfWorkFactory>();

            Keys = [.. keys];
        }

        public async Task<IActionResult> CreateAsync(
            TDto create,
            ODataQueryOptions<TDto> queryOptions,
            CancellationToken cancellationToken = default)
        {
            queryOptions.EnsureValidForSingle();

            using var uow = _uowFactory.Create(new() { SccopeMode = UnitOfWorkScopeMode.RequestScope });
            var repository = uow.Repository<TEntity>();

            var validator = _modelValidatorFactory.GetValidator<TDto>();
            await validator.ValidateAsync(create, cancellationToken);
            var record = _mapper.Map<TEntity>(create);

            foreach (var beforeSave in _onCreating)
            {
                await beforeSave(record, create);
            }

            var dbValidator = _modelValidatorFactory.GetValidator<TEntity>();
            await dbValidator.ValidateAsync(record, cancellationToken);

            repository.Add(record);

            await uow.SaveChangesAsync(cancellationToken: cancellationToken);

            record = await repository.ReloadAsync(record, cancellationToken);

            var finished = _mapper
                .MapExplicitly(record)
                .To<TDto>()
                .ApplyQueryOptions(queryOptions)
                .Execute();

            return _controller.Created(finished);
        }

        public async Task<IActionResult> DeleteAsync(
            CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.Create(new() { SccopeMode = UnitOfWorkScopeMode.RequestScope });
            var repository = uow.Repository<TEntity>();

            var record = await _controller.LoadRecordAsync(uow, Keys, cancellationToken: cancellationToken);

            repository.Remove(record);

            await uow.SaveChangesAsync(cancellationToken: cancellationToken);

            return _controller.NoContent();
        }

        public IReferenceRequestBuilder<TEntity, TDto, TFEntity, TFDto> ForReference<TFEntity, TFDto, TFKey>(
            Func<TEntity, ICollection<TFEntity>> findCollection,
            params KeyMapping<TFEntity, TFDto, TFKey>[] fkeys)
            where TFEntity : class, IEntity
            where TFDto : class, new()
        {
            return new SetReferenceRequestBuilder<TEntity, TDto, TKey, TFEntity, TFDto, TFKey>(
                _controller, [.. Keys], findCollection, fkeys);
        }

        public async Task<IActionResult> GetAsync(
            ODataQueryOptions<TDto> queryOptions,
            CancellationToken cancellationToken = default)
        {
            queryOptions.EnsureValidForSingle();

            using var uow = _uowFactory.Create(new() { SccopeMode = UnitOfWorkScopeMode.RequestScope });
            var repository = uow.Repository<TEntity>();

            var record = await _controller.LoadRecordAsync(uow, Keys, cancellationToken: cancellationToken);

            var finished = _mapper
                .MapExplicitly(record)
                .To<TDto>()
                .ApplyQueryOptions(queryOptions)
                .Execute();

            return _controller.Ok(finished);
        }

        public IKeylessRequestBuilder<TEntity, TDto> OnCreating(
            Func<TEntity, TDto, Task> beforeSave)
        {
            _onCreating.Add(beforeSave);
            return this;
        }

        public IKeyedRequestBuilder<TEntity, TDto> OnUpdating(
            Func<TEntity, TDto, Task> beforeSave)
        {
            _onUpdating.Add(beforeSave);
            return this;
        }

        public async Task<IActionResult> PatchAsync(
            Delta<TDto> patch,
            ODataQueryOptions<TDto> queryOptions,
            CancellationToken cancellationToken = default)
        {
            queryOptions.EnsureValidForSingle();

            using var uow = _uowFactory.Create(new() { SccopeMode = UnitOfWorkScopeMode.RequestScope });
            var repository = uow.Repository<TEntity>();

            var testRecord = new TDto();
            patch.Patch(testRecord);

            var record = await _controller.LoadRecordAsync(uow, Keys, cancellationToken: cancellationToken);

            var recordAsDto = _mapper
                .Map<TDto>(record);

            patch.Patch(recordAsDto);

            var validator = _modelValidatorFactory.GetValidator<TDto>();
            await validator.ValidateAsync(recordAsDto, cancellationToken);

            _mapper
                .MergeInto(record)
                .Using(recordAsDto)
                .Execute();

            foreach (var beforeSave in _onUpdating)
            {
                await beforeSave(record, recordAsDto);
            }

            var dbValidator = _modelValidatorFactory.GetValidator<TEntity>();
            await dbValidator.ValidateAsync(record, cancellationToken);

            await uow.SaveChangesAsync(cancellationToken: cancellationToken);

            var finished = _mapper
                .MapExplicitly(record)
                .To<TDto>()
                .ApplyQueryOptions(queryOptions)
                .Execute();

            return _controller.Ok(finished);
        }

        public async Task<IActionResult> SearchAsync(
            ODataQueryOptions<TDto> queryOptions,
            CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.Create(new() { SccopeMode = UnitOfWorkScopeMode.RequestScope });
            var repository = uow.Repository<TEntity>();

            var finished = await _mapper
                .MapExplicitly(repository.Data)
                .To<TDto>()
                .ApplyQueryOptions(queryOptions)
                .ExecuteAsync(cancellationToken);

            return _controller.Ok(finished);
        }
    }

    protected interface IReferenceRequestBuilder<TEntity, TDto, TFEntity, TFDto>
        where TEntity : class
        where TDto : class, new()
        where TFEntity : class
        where TFDto : class, new()
    {
        Task<IActionResult> CreateAsync(CancellationToken cancellationToken = default);

        Task<IActionResult> DeleteAsync(CancellationToken cancellationToken = default);

        IReferenceRequestBuilder<TEntity, TDto, TFEntity, TFDto> OnCreating(Func<TEntity, TDto, TFEntity, TFDto, Task> beforeSave);

        IReferenceRequestBuilder<TEntity, TDto, TFEntity, TFDto> OnDeleting(Func<TEntity, TDto, TFEntity, TFDto, Task> beforeSave);
    }

    private class SetReferenceRequestBuilder<TEntity, TDto, TKey, TFEntity, TFDto, TFKey> :
        IReferenceRequestBuilder<TEntity, TDto, TFEntity, TFDto>
        where TEntity : class, IEntity
        where TDto : class, new()
        where TFEntity : class, IEntity
        where TFDto : class, new()
    {
        private readonly AutoODataController _controller;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkFactory _uowFactory;

        private readonly List<Func<TEntity, TDto, TFEntity, TFDto, Task>> _onCreating = [];
        private readonly List<Func<TEntity, TDto, TFEntity, TFDto, Task>> _onDeleting = [];

        public List<KeyMapping<TEntity, TDto, TKey>> Keys { get; }

        public Func<TEntity, ICollection<TFEntity>> FindCollection { get; }

        public List<KeyMapping<TFEntity, TFDto, TFKey>> FKeys { get; }

        public SetReferenceRequestBuilder(
            AutoODataController controller,
            KeyMapping<TEntity, TDto, TKey>[] keys,
            Func<TEntity, ICollection<TFEntity>> findCollection,
            params KeyMapping<TFEntity, TFDto, TFKey>[] fkeys)
        {
            _controller = controller;
            _mapper = controller._serviceProvider.GetRequiredService<IMapper>();
            _uowFactory = _controller._serviceProvider.GetRequiredService<IUnitOfWorkFactory>();

            Keys = [.. keys];
            FindCollection = findCollection;
            FKeys = [.. fkeys];
        }

        public async Task<IActionResult> CreateAsync(
            CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.Create(new() { SccopeMode = UnitOfWorkScopeMode.RequestScope });
            var repository = uow.Repository<TEntity>();

            var record = await _controller.LoadRecordAsync(uow, Keys, cancellationToken: cancellationToken);

            var foreignRepository = uow.Repository<TFEntity>();

            var foreignRecord = await _controller.LoadRecordAsync(uow, FKeys, target: ErrorConstants.RequestTarget.Body, cancellationToken: cancellationToken);

            FindCollection(record).Add(foreignRecord);

            var recordAsDto = _mapper
                .Map<TDto>(record);

            var foreignRecordAsDto = _mapper
                .Map<TFDto>(foreignRecord);

            foreach (var beforeSave in _onCreating)
            {
                await beforeSave(record, recordAsDto, foreignRecord, foreignRecordAsDto);
            }

            await uow.SaveChangesAsync(cancellationToken);

            return _controller.NoContent();
        }

        public async Task<IActionResult> DeleteAsync(
            CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.Create(new() { SccopeMode = UnitOfWorkScopeMode.RequestScope });
            var repository = uow.Repository<TEntity>();

            var record = await _controller.LoadRecordAsync(uow, Keys, cancellationToken: cancellationToken);

            var foreignRepository = uow.Repository<TFEntity>();

            var foreignRecord = await _controller.LoadRecordAsync(uow, FKeys, target: ErrorConstants.RequestTarget.Query, cancellationToken: cancellationToken);

            FindCollection(record).Remove(foreignRecord);

            var recordAsDto = _mapper
                .Map<TDto>(record);

            var foreignRecordAsDto = _mapper
                .Map<TFDto>(foreignRecord);

            foreach (var beforeSave in _onDeleting)
            {
                await beforeSave(record, recordAsDto, foreignRecord, foreignRecordAsDto);
            }

            await uow.SaveChangesAsync(cancellationToken);

            return _controller.NoContent();
        }

        public IReferenceRequestBuilder<TEntity, TDto, TFEntity, TFDto> OnCreating(
            Func<TEntity, TDto, TFEntity, TFDto, Task> beforeSave)
        {
            _onCreating.Add(beforeSave);
            return this;
        }

        public IReferenceRequestBuilder<TEntity, TDto, TFEntity, TFDto> OnDeleting(
            Func<TEntity, TDto, TFEntity, TFDto, Task> beforeSave)
        {
            _onDeleting.Add(beforeSave);
            return this;
        }
    }
}
