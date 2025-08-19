using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Extensions;

public static class ODataControllerExtensions
{
    public static bool TryParseRelatedKey<TKey>(
        this ODataController @this,
        Uri link,
        [NotNullWhen(true)] out TKey? relatedKey,
        int index = 0)
    {
        relatedKey = default;

        var model = @this.Request
            .GetRouteServices()
            .GetRequiredService<IEdmModel>();

        var serviceRoot = @this.Request
            .CreateODataLink();

        var uriParser = new ODataUriParser(model, new Uri(serviceRoot), link);

        var odataPath = uriParser.ParsePath();
        var keySegment = odataPath
            .OfType<KeySegment>()
            .LastOrDefault();

        if (keySegment is null || keySegment.Keys.ElementAt(index).Value is not TKey parsedKey)
        {
            return false;
        }

        relatedKey = parsedKey;
        return true;
    }
}
