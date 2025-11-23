using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using System.Net;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Aspects;

internal class SearchAuthenticationAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "authenticated";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ODataCapability.Search))
            yield break;

        yield return new AspectCase($"search:auth:200", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            var response = await client.GetAsync(specification.BaseRoute);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.Forbidden));
        });

        yield return new AspectCase($"search:nocap:403", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString(), capabilities: "");

            var response = await client.GetAsync(specification.BaseRoute);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        });

        yield return new AspectCase($"search:anon:401", async context =>
        {
            var client = context.GetAnonymousClient();

            var response = await client.GetAsync(specification.BaseRoute);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        });
    }
}

internal class GetAuthenticationAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "authenticated";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ODataCapability.Get))
            yield break;

        yield return new AspectCase($"get:auth:200", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForRead);
            var response = await client.GetAsync(specification.BaseRoute + specification.Path(id));

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.Forbidden));
        });

        yield return new AspectCase($"get:nocap:403", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString(), capabilities: "");

            var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForRead);
            var response = await client.GetAsync(specification.BaseRoute + specification.Path(id));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        });

        yield return new AspectCase($"get:anon:401", async context =>
        {
            var client = context.GetAnonymousClient();

            var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForRead);
            var response = await client.GetAsync(specification.BaseRoute + specification.Path(id));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        });
    }
}

internal class CreateAuthenticationAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "authenticated";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ODataCapability.Create))
            yield break;

        yield return new AspectCase($"create:auth:201", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            var body = specification.Create();
            var response = await client.PostAsync(specification.BaseRoute, body);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.Forbidden));
        });

        yield return new AspectCase($"create:nocap:403", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString(), capabilities: "");

            var body = specification.Create();
            var response = await client.PostAsync(specification.BaseRoute, body);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        });

        yield return new AspectCase($"create:anon:401", async context =>
        {
            var client = context.GetAnonymousClient();

            var body = specification.Create();
            var response = await client.PostAsync(specification.BaseRoute, body);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        });
    }
}

internal class PatchAuthenticationAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "authenticated";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ODataCapability.Update))
            yield break;

        yield return new AspectCase($"patch:auth:200", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForUpdate);
            var body = specification.Update(entity);
            var response = await client.PatchAsync(specification.BaseRoute + specification.Path(id), body);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.Forbidden));
        });

        yield return new AspectCase($"patch:nocap:403", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString(), capabilities: "");

            var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForUpdate);
            var body = specification.Update(entity);
            var response = await client.PatchAsync(specification.BaseRoute + specification.Path(id), body);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        });

        yield return new AspectCase($"patch:anon:401", async context =>
        {
            var client = context.GetAnonymousClient();

            var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForUpdate);
            var body = specification.Update(entity);
            var response = await client.PatchAsync(specification.BaseRoute + specification.Path(id), body);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        });
    }
}

internal class DeleteAuthenticationAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "authenticated";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ODataCapability.Delete))
            yield break;

        yield return new AspectCase($"delete:auth:204", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForUpdate);
            var response = await client.DeleteAsync(specification.BaseRoute + specification.Path(id));

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.Forbidden));
        });

        yield return new AspectCase($"delete:nocap:403", async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString(), capabilities: "");

            var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForUpdate);
            var response = await client.DeleteAsync(specification.BaseRoute + specification.Path(id));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        });

        yield return new AspectCase($"delete:anon:401", async context =>
        {
            var client = context.GetAnonymousClient();

            var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForUpdate);
            var response = await client.DeleteAsync(specification.BaseRoute + specification.Path(id));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        });
    }
}
