using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Specifications;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Text;

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

        yield return new AspectCase($"search:group:filtered", async context =>
        {
            var user1Id = await GroupFilterMeta.GetUserIdAsync("search:1");
            var user2Id = await GroupFilterMeta.GetUserIdAsync("search:2");

            var client1 = context.GetClientWithCapabilities(user1Id.ToString());
            var client2 = context.GetClientWithCapabilities(user2Id.ToString());

            var record1 = await specification.Records.AcquireAsync(context, AcquirePurpose.ForRead, user1Id);

            var response1 = await client1.GetAsync(specification.BaseRoute + "?$count=true");
            var response2 = await client2.GetAsync(specification.BaseRoute + "?$count=true");

            Assert.Multiple(() =>
            {
                Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            });

            var (_, _, count1) = await ODataReader.ReadListRawAsync(response1);
            var (_, _, count2) = await ODataReader.ReadListRawAsync(response2);

            Assert.Multiple(() =>
            {
                Assert.That(count1, Is.EqualTo(count2 + 1));
                Assert.That(count2, Is.EqualTo(count1 - 1));
            });
        });
    }
}

internal class GetGroupFilterAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "groupfilter";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ResourceFunctionality.GroupFiltering))
            yield break;

        if (!specification.Capabilities.Contains(ResourceFunctionality.Get))
            yield break;

        yield return new AspectCase($"get:group:filtered", async context =>
        {
            var user1Id = await GroupFilterMeta.GetUserIdAsync("get:1");
            var user2Id = await GroupFilterMeta.GetUserIdAsync("get:2");

            var client1 = context.GetClientWithCapabilities(user1Id.ToString());
            var client2 = context.GetClientWithCapabilities(user2Id.ToString());

            var record1 = await specification.Records.AcquireAsync(context, AcquirePurpose.ForRead, user1Id);

            var response1 = await client1.GetAsync(specification.BaseRoute + specification.Path(record1.Id));
            var response2 = await client2.GetAsync(specification.BaseRoute + specification.Path(record1.Id));

            Assert.Multiple(() =>
            {
                Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            });
        });
    }
}

internal class PatchGroupFilterAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "groupfilter";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ResourceFunctionality.GroupFiltering))
            yield break;

        if (!specification.Capabilities.Contains(ResourceFunctionality.Update))
            yield break;

        yield return new AspectCase($"patch:group:filtered", async context =>
        {
            var user1Id = await GroupFilterMeta.GetUserIdAsync("patch:1");
            var user2Id = await GroupFilterMeta.GetUserIdAsync("patch:2");

            var client1 = context.GetClientWithCapabilities(user1Id.ToString());
            var client2 = context.GetClientWithCapabilities(user2Id.ToString());

            var record1 = await specification.Records.AcquireAsync(context, AcquirePurpose.ForRead, user1Id);
            var payload = specification.Update(record1.Entity);

            var response1 = await client1.PatchAsync(specification.BaseRoute + specification.Path(record1.Id), payload);
            var response2 = await client2.PatchAsync(specification.BaseRoute + specification.Path(record1.Id), payload);

            Assert.Multiple(() =>
            {
                Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            });
        });
    }
}

internal class DeleteGroupFilterAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "groupfilter";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ResourceFunctionality.GroupFiltering))
            yield break;

        if (!specification.Capabilities.Contains(ResourceFunctionality.Delete))
            yield break;

        yield return new AspectCase($"delete:group:filtered", async context =>
        {
            var user1Id = await GroupFilterMeta.GetUserIdAsync("delete:1");
            var user2Id = await GroupFilterMeta.GetUserIdAsync("delete:2");

            var client1 = context.GetClientWithCapabilities(user1Id.ToString());
            var client2 = context.GetClientWithCapabilities(user2Id.ToString());

            var record1 = await specification.Records.AcquireAsync(context, AcquirePurpose.ForRead, user1Id);
            var payload = specification.Update(record1.Entity);

            var response1 = await client1.DeleteAsync(specification.BaseRoute + specification.Path(record1.Id));
            var response2 = await client2.DeleteAsync(specification.BaseRoute + specification.Path(record1.Id));

            Assert.Multiple(() =>
            {
                Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
                Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            });
        });
    }
}

internal static class GroupFilterMeta
{
    private static ConcurrentDictionary<string, Task<Guid>> _usersByGroup = [];

    public static async Task<Guid> GetUserIdAsync(
        string groupName)
    {
        var task = _usersByGroup.GetOrAdd(
            groupName.Trim().ToLowerInvariant(),
            _ => CreateUserAsync());

        return await task;
    }

    private static async Task<Guid> CreateUserAsync()
    {
        var (_, groupId) = await GroupsControllerV1Spec.Instance.Records.AcquireAsync(FixtureContext.Instance, AcquirePurpose.ForUpdate);

        var (_, userId) = await UsersControllerV1Spec.Instance.Records.AcquireAsync(FixtureContext.Instance, AcquirePurpose.ForUpdate);

        using var client = FixtureContext.Instance.GetClientWithCapabilities(Setup.OwnerUserId.ToString());
        var response = await client.PatchAsync(
            UsersControllerV1Spec.Instance.BaseRoute + UsersControllerV1Spec.Instance.Path(userId),
            JsonContent.Create(new
            {
                groupId,
            }));

        Assert.That(response.IsSuccessStatusCode, Is.True);

        response = await client.GetAsync(
            RolesControllerV1Spec.Instance.BaseRoute + "?$filter=name eq 'Group Owner'");

        var (_, items, _) = await ODataReader.ReadListRawAsync(response);
        var roleId = items[0].GetProperty("id").GetInt32();

        response = await client.PostAsync(
            UsersControllerV1Spec.Instance.BaseRoute + UsersControllerV1Spec.Instance.Path(userId) + "/roles/$ref",
            new StringContent($"{{\"@odata.id\":\"https://localhost/api/v1/roles/{roleId}\"}}", Encoding.UTF8, "application/json"));

        Assert.That(response.IsSuccessStatusCode, Is.True);

        return userId;
    }
}
