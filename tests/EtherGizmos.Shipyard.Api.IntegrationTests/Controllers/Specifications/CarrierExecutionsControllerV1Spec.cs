using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Api.Enums;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.Shipyard.Controllers.Specifications;

public class CarrierExecutionsControllerV1Spec : IODataResourceSpec<CarrierExecutionDTO, int>
{
    public static CarrierExecutionsControllerV1Spec Instance { get; }

    static CarrierExecutionsControllerV1Spec()
    {
        Instance = new();
    }

    public string BaseRoute => "api/v1/carrierExecutions";

    public IReadOnlySet<ResourceFunctionality> Capabilities =>
        new HashSet<ResourceFunctionality>()
        {
            //Actions
            ResourceFunctionality.Search,
            ResourceFunctionality.Get,

            //Query options
            ResourceFunctionality.QueryCount,
            ResourceFunctionality.QueryExpand,
            ResourceFunctionality.QueryFilter,
            ResourceFunctionality.QueryOrderBy,
            ResourceFunctionality.QuerySelect,
            ResourceFunctionality.QuerySkip,
            ResourceFunctionality.QueryTop,
        };

    public Func<CarrierExecutionDTO, int> Identity => carrier => carrier.Id;

    public Func<int, string> Path => id => $"({id})";

    public IRecordSource<CarrierExecutionDTO, int> Records => new CarrierExecutionsControllerV1Source(this);

    public HttpContent Create() => throw new NotImplementedException();

    public HttpContent Update(CarrierExecutionDTO entity) => throw new NotImplementedException();

    private class CarrierExecutionsControllerV1Source : IRecordSource<CarrierExecutionDTO, int>
    {
        private readonly IODataResourceSpec<CarrierExecutionDTO, int> _specification;

        public CarrierExecutionsControllerV1Source(
            IODataResourceSpec<CarrierExecutionDTO, int> specification)
        {
            _specification = specification;
        }

        public async Task<(CarrierExecutionDTO Entity, int Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose,
            Guid? createdByUserId = null)
        {
            var (_, id) = await CarriersControllerV1Spec.Instance.Records.AcquireAsync(context, purpose, createdByUserId);
            var (_, packageId) = await PackagesControllerV1Spec.Instance.Records.AcquireAsync(context, purpose, createdByUserId);

            var uowFactory = Setup.Services.GetRequiredService<IUnitOfWorkFactory>();
            using var uow = uowFactory.Create();

            var executionRepo = uow.Repository<CarrierExecution>();

            var execution = new CarrierExecution()
            {
                CarrierId = id,
                PackageId = packageId,
                StepCount = 1,
                ExecutionStatus = ExecutionStatusType.Successful,
            };

            executionRepo.Add(execution);

            await uow.SaveChangesAsync();

            return (new()
            {
                Id = execution.Id,
                StepCount = 1,
                ExecutionStatusType = ExecutionStatusTypeDTO.Successful,
            }, execution.Id);
        }
    }
}
