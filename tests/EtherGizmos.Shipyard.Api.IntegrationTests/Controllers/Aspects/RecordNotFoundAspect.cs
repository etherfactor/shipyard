using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using System.Net;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Aspects;

internal class GetRecordNotFoundAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "notfound";

    public IEnumerable<AspectCase> Build(IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ODataCapability.Get))
            yield break;

        yield return new AspectCase($"get:notexist:404", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            var response = await client.GetAsync(specification.BaseRoute + specification.Path(default!));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }
}

internal class PatchRecordNotFoundAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "notfound";

    public IEnumerable<AspectCase> Build(IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ODataCapability.Get))
            yield break;

        yield return new AspectCase($"patch:notexist:404", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            var body = specification.Create();
            var response = await client.PatchAsync(specification.BaseRoute + specification.Path(default!), body);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }
}

internal class DeleteRecordNotFoundAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "notfound";

    public IEnumerable<AspectCase> Build(IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ODataCapability.Get))
            yield break;

        yield return new AspectCase($"delete:notexist:404", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            var response = await client.DeleteAsync(specification.BaseRoute + specification.Path(default!));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }
}
