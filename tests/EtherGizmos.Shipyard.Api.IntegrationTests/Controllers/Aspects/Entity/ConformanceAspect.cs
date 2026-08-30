using EtherGizmos.Shipyard.Abstractions;
using System.Net;

namespace EtherGizmos.Shipyard.Controllers.Aspects.Entity;

internal class SearchConformanceAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "conform";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ResourceFunctionality.Search))
            yield break;

        yield return new AspectCase($"search:conform", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            var response = await client.GetAsync(specification.BaseRoute);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });
    }
}

internal class GetConformanceAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "conform";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ResourceFunctionality.Get))
            yield break;

        yield return new AspectCase($"get:conform", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForRead);
            var response = await client.GetAsync(specification.BaseRoute + specification.Path(id));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });
    }
}

internal class CreateConformanceAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "conform";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ResourceFunctionality.Create))
            yield break;

        yield return new AspectCase($"create:conform", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            var body = specification.Create();
            var response = await client.PostAsync(specification.BaseRoute, body);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        });
    }
}

internal class PatchConformanceAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "conform";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ResourceFunctionality.Update))
            yield break;

        yield return new AspectCase($"patch:conform", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForUpdate);
            var body = specification.Update(entity);
            var response = await client.PatchAsync(specification.BaseRoute + specification.Path(id), body);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });
    }
}

internal class DeleteConformanceAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "conform";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ResourceFunctionality.Delete))
            yield break;

        yield return new AspectCase($"delete:conform", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForUpdate);
            var response = await client.DeleteAsync(specification.BaseRoute + specification.Path(id));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            response = await client.GetAsync(specification.BaseRoute + specification.Path(id));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }
}
