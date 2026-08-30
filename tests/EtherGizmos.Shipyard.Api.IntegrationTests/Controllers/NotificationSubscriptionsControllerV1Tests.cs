using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Controllers.Aspects.Entity;
using EtherGizmos.Shipyard.Controllers.Specifications;

namespace EtherGizmos.Shipyard.Controllers;

internal class NotificationSubscriptionsControllerV1Tests : IntegrationTestBase
{
    public static IEnumerable<AspectCase> All
        => EntityAspects.BuildAll(NotificationSubscriptionsControllerV1Spec.Instance);

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
        => await NotificationSubscriptionsControllerV1Spec.Instance.Records.AcquireAsync(FixtureContext.Instance, AcquirePurpose.ForRead);

    [TestCaseSource(nameof(All))]
    public async Task Aspect(AspectCase c)
        => await c.TestAsync(FixtureContext.Instance);
}
