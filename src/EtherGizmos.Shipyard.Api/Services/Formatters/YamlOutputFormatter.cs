using Microsoft.AspNetCore.Mvc.Formatters;
using System.Reflection;
using System.Text;
using VYaml.Annotations;
using VYaml.Serialization;

namespace EtherGizmos.Shipyard.Api.Services.Formatters;

public class YamlOutputFormatter : TextOutputFormatter
{
    public YamlOutputFormatter()
    {
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
        SupportedMediaTypes.Add("application/yaml");
    }

    protected override bool CanWriteType(
        Type? type)
    {
        if (type is null)
            return false;

        var attribute = type.GetCustomAttribute<YamlObjectAttribute>();
        return attribute is not null;
    }

    public override async Task WriteResponseBodyAsync(
        OutputFormatterWriteContext context,
        Encoding selectedEncoding)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(selectedEncoding);

        var response = context.HttpContext.Response;
        using var writer = context.WriterFactory(response.Body, selectedEncoding);

        typeof(YamlOutputFormatter)
            .GetMethod(nameof(WriteObject), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod([context.ObjectType!])
            .Invoke(null, [writer, context.Object]);

        await writer.FlushAsync();
    }

    private static void WriteObject<TObject>(
        TextWriter writer,
        TObject value)
    {
        var bytes = YamlSerializer.Serialize(value, new() { DefaultIgnoreCondition = YamlIgnoreCondition.WhenWritingNull });
        var asString = Encoding.UTF8.GetString(bytes.Span);
        writer.Write(asString);
    }
}
