using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Aspects.Entity;
using EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Specifications;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers;

internal class GroupsControllerV1Tests
{
    public static IEnumerable<AspectCase> All
        => EntityAspects.BuildAll(GroupsControllerV1Spec.Instance);

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
        => await GroupsControllerV1Spec.Instance.Records.AcquireAsync(FixtureContext.Instance, AcquirePurpose.ForRead);

    [TestCaseSource(nameof(All))]
    public async Task Aspect(AspectCase c)
        => await c.TestAsync(FixtureContext.Instance);
}
