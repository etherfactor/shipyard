using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Aspects.Entity;
using EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Specifications;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers;

internal class CarrierExecutionsControllerV1Tests
{
    public static IEnumerable<AspectCase> All
        => EntityAspects.BuildAll(CarrierExecutionsControllerV1Spec.Instance);

    [TestCaseSource(nameof(All))]
    public async Task Aspect(AspectCase c)
        => await c.TestAsync(FixtureContext.Instance);
}
