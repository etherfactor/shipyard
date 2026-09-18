using EtherGizmos.Common.Extensions;
using EtherGizmos.Shipyard.Abstractions;
using System.Net;
using System.Text.Json;

namespace EtherGizmos.Shipyard.Controllers.Aspects.Entity;

internal class SearchSelectOptionAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "search:$select";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ResourceFunctionality.Search))
            yield break;

        yield return new AspectCase(Name, async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            //Grab any property, so we don't assume a property that may not exist
            var property = typeof(TEntity).GetProperties().First();
            var propertyName = property.Name.ToFirstLower();

            await specification.Records.AcquireAsync(context, AcquirePurpose.ForRead);
            var response = await client.GetAsync(specification.BaseRoute + $"?$select={propertyName}&$top=1");

            await SearchSet.ValidateSetAsync<TEntity, TId>(response, propertyName);
        });
    }
}

internal class GetSelectOptionAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "get:$select";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ResourceFunctionality.Get))
            yield break;

        yield return new AspectCase(Name, async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            //Grab any property, so we don't assume a property that may not exist
            var property = typeof(TEntity).GetProperties().First();
            var propertyName = property.Name.ToFirstLower();

            var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForRead);
            var response = await client.GetAsync(specification.BaseRoute + specification.Path(id) + $"?$select={propertyName}");

            await SearchSet.ValidateSingleAsync<TEntity, TId>(response, propertyName);
        });
    }
}

internal class CreateSelectOptionAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "create:$select";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ResourceFunctionality.Create))
            yield break;

        yield return new AspectCase(Name, async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            //Grab any property, so we don't assume a property that may not exist
            var property = typeof(TEntity).GetProperties().First();
            var propertyName = property.Name.ToFirstLower();

            var body = specification.Create();
            var response = await client.PostAsync(specification.BaseRoute + $"?$select={propertyName}", body);

            await SearchSet.ValidateSingleAsync<TEntity, TId>(response, propertyName);
        });
    }
}

internal class PatchSelectOptionAspect<TEntity, TId>
    : IAspect<TEntity, TId>
    where TEntity : class, new()
{
    public string Name => "patch:$select";

    public IEnumerable<AspectCase> Build(
        IODataResourceSpec<TEntity, TId> specification)
    {
        if (!specification.Capabilities.Contains(ResourceFunctionality.Update))
            yield break;

        yield return new AspectCase(Name, async context =>
        {
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());

            //Grab any property, so we don't assume a property that may not exist
            var property = typeof(TEntity).GetProperties().First();
            var propertyName = property.Name.ToFirstLower();

            var (entity, id) = await specification.Records.AcquireAsync(context, AcquirePurpose.ForUpdate);
            var body = specification.Update(entity);
            var response = await client.PatchAsync(specification.BaseRoute + specification.Path(id) + $"?$select={propertyName}", body);

            await SearchSet.ValidateSingleAsync<TEntity, TId>(response, propertyName);
        });
    }
}

internal static class SearchSet
{
    public static async Task ValidateSingleAsync<TEntity, TId>(
        HttpResponseMessage response,
        string propertyName)
    {
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.Created));

        var (root, item) = await ODataReader.ReadSingleRawAsync(response);

        ValidateInner<TEntity, TId>(item, propertyName);
    }

    public static async Task ValidateSetAsync<TEntity, TId>(
        HttpResponseMessage response,
        string propertyName)
    {
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var (root, items, count) = await ODataReader.ReadListRawAsync(response);
        var element = items[0];

        ValidateInner<TEntity, TId>(element, propertyName);
    }

    private static void ValidateInner<TEntity, TId>(
        JsonElement element,
        string propertyName)
    {
        JsonShapeAssert.HasOnlyProps(element, [propertyName]);
    }
}
