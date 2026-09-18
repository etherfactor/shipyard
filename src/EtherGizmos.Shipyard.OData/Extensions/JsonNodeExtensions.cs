using System.Text.Json;
using System.Text.Json.Nodes;

namespace EtherGizmos.Shipyard.Extensions;

internal static class JsonNodeExtensions
{
    extension(JsonNode @this)
    {
        public bool IsODataError()
        {
            if (@this.GetValueKind() != JsonValueKind.Object)
                return false;

            var obj = @this.AsObject();
            var error = obj["error"];
            return error is not null
                && error["code"] is not null;
        }
    }
}
