using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Aspects.Entity;
using EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Specifications;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers;

internal class PackagesControllerV1Tests
{
    public static IEnumerable<AspectCase> All
        => EntityAspects.BuildAll(PackagesControllerV1Spec.Instance);

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
        => await PackagesControllerV1Spec.Instance.Records.AcquireAsync(FixtureContext.Instance, AcquirePurpose.ForRead);

    [TestCaseSource(nameof(All))]
    public async Task Aspect(AspectCase c)
        => await c.TestAsync(FixtureContext.Instance);
}
