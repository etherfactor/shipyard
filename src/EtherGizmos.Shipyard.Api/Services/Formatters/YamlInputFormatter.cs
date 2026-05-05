using Microsoft.AspNetCore.Mvc.Formatters;
using System.Reflection;
using System.Text;
using VYaml.Annotations;
using VYaml.Serialization;

namespace EtherGizmos.Shipyard.Services.Formatters;

public class YamlInputFormatter : TextInputFormatter
{
    public YamlInputFormatter()
    {
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
        SupportedMediaTypes.Add("application/yaml");
    }

    protected override bool CanReadType(
        Type type)
    {
        var attribute = type.GetCustomAttribute<YamlObjectAttribute>();
        return attribute is not null;
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(
        InputFormatterContext context,
        Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(encoding);

        var request = context.HttpContext.Request;
        using var reader = context.ReaderFactory(request.Body, encoding);

        var content = await reader.ReadToEndAsync();

        var result = typeof(YamlInputFormatter)
            .GetMethod(nameof(ReadObject), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod([context.ModelType])
            .Invoke(null, [content])!;

        return InputFormatterResult.Success(result);
    }

    private static TObject ReadObject<TObject>(
        string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var asObject = YamlSerializer.Deserialize<TObject>(bytes);
        return asObject;
    }
}
