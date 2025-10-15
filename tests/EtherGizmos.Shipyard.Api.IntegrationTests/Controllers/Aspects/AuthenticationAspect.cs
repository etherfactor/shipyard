using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Aspects;

internal class AuthenticationAspect<TEntity, TId> : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "authenticated";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (specification.Capabilities.Contains(ODataCapability.Search))
        {
            yield return new AspectCase($"search:auth:2XX", async context =>
            {
                var client = context.GetClientAsRole("123", 1);

                var response = await client.GetAsync(specification.BaseRoute);

                Assert.That(response.IsSuccessStatusCode, Is.True);
            });

            yield return new AspectCase($"search:anon:4XX", async context =>
            {
                var client = context.GetAnonymousClient();

                var response = await client.GetAsync(specification.BaseRoute);

                Assert.That(response.IsSuccessStatusCode, Is.False);
            });
        }

        if (specification.Capabilities.Contains(ODataCapability.Get))
        {
            yield return new AspectCase($"get:auth:2XX", async context =>
            {
                var client = context.GetClientAsRole("123", 1);

                var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForRead);
                var response = await client.GetAsync(specification.BaseRoute + specification.Path(entity));

                Assert.That(response.IsSuccessStatusCode, Is.True);
            });

            yield return new AspectCase($"get:anon:4XX", async context =>
            {
                var client = context.GetAnonymousClient();

                var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForRead);
                var response = await client.GetAsync(specification.BaseRoute + specification.Path(entity));

                Assert.That(response.IsSuccessStatusCode, Is.False);
            });
        }

        if (specification.Capabilities.Contains(ODataCapability.Create))
        {
            yield return new AspectCase($"create:auth:2XX", async context =>
            {
                var client = context.GetClientAsRole("123", 1);

                var body = specification.Create();
                var response = await client.PostAsync(specification.BaseRoute, body);

                Assert.That(response.IsSuccessStatusCode, Is.True);
            });

            yield return new AspectCase($"create:anon:4XX", async context =>
            {
                var client = context.GetAnonymousClient();

                var body = specification.Create();
                var response = await client.PostAsync(specification.BaseRoute, body);

                Assert.That(response.IsSuccessStatusCode, Is.False);
            });
        }

        if (specification.Capabilities.Contains(ODataCapability.Update))
        {
            yield return new AspectCase($"patch:auth:2XX", async context =>
            {
                var client = context.GetClientAsRole("123", 1);

                var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForUpdate);
                var body = specification.Update(entity);
                var response = await client.PatchAsync(specification.BaseRoute + specification.Path(entity), body);

                Assert.That(response.IsSuccessStatusCode, Is.True);
            });

            yield return new AspectCase($"patch:anon:4XX", async context =>
            {
                var client = context.GetAnonymousClient();

                var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForUpdate);
                var body = specification.Update(entity);
                var response = await client.PatchAsync(specification.BaseRoute + specification.Path(entity), body);

                Assert.That(response.IsSuccessStatusCode, Is.False);
            });
        }

        if (specification.Capabilities.Contains(ODataCapability.Update))
        {
            yield return new AspectCase($"delete:auth:2XX", async context =>
            {
                var client = context.GetClientAsRole("123", 1);

                var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForUpdate);
                var response = await client.DeleteAsync(specification.BaseRoute + specification.Path(entity));

                Assert.That(response.IsSuccessStatusCode, Is.True);
            });

            yield return new AspectCase($"delete:anon:4XX", async context =>
            {
                var client = context.GetAnonymousClient();

                var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForUpdate);
                var response = await client.DeleteAsync(specification.BaseRoute + specification.Path(entity));

                Assert.That(response.IsSuccessStatusCode, Is.False);
            });
        }
    }
}
