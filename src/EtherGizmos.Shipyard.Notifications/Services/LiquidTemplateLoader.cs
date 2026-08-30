using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace EtherGizmos.Shipyard.Services;

internal class LiquidTemplateLoader : ITemplateLoader
{
    public string? GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        => $"{templateName}.mjml.html";

    public string? Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        var assembly = typeof(LiquidTemplateLoader).Assembly;
        using var stream = assembly.GetManifestResourceStream($"EtherGizmos.Shipyard.Events.Base.Templates.{templatePath}");
        if (stream is null)
            return null;

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public async ValueTask<string?> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        var assembly = typeof(LiquidTemplateLoader).Assembly;
        using var stream = assembly.GetManifestResourceStream($"EtherGizmos.Shipyard.Events.Base.Templates.{templatePath}");
        if (stream is null)
            return null;

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(context.CancellationToken);
    }
}
