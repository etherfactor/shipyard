using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using System.Net;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Aspects.Entity;

internal class SearchGroupFilterAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "groupfilter";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ResourceFunctionality.GroupFiltering))
            yield break;

        if (!specification.Capabilities.Contains(ResourceFunctionality.Search))
            yield break;

        yield return new AspectCase($"search:group:visible", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            var response = await client.GetAsync(specification.BaseRoute);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.Forbidden));
        });

        yield return new AspectCase($"search:notgroup:notvisible", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString(), capabilities: "");

            var response = await client.GetAsync(specification.BaseRoute);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        });
    }
}
