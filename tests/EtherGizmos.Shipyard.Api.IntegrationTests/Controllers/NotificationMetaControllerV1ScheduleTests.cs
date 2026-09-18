using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Controllers.Aspects.Entity;
using EtherGizmos.Shipyard.Controllers.Specifications;

namespace EtherGizmos.Shipyard.Controllers;

internal class NotificationMetaControllerV1ScheduleTests : IntegrationTestBase
{
    public static IEnumerable<AspectCase> All
        => EntityAspects.BuildAll(NotificationMetaControllerV1ScheduleSpec.Instance);

    [TestCaseSource(nameof(All))]
    public async Task Aspect(AspectCase c)
        => await c.TestAsync(FixtureContext.Instance);
}
