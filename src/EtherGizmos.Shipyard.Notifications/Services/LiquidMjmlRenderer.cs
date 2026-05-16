using Mjml.Net;
using Scriban;
using Scriban.Runtime;
using System.Text.RegularExpressions;

namespace EtherGizmos.Shipyard.Services;

internal static partial class LiquidMjmlRenderer
{
    public static string Render(
        string templateName,
        object model)
    {
        var liquid = LoadTemplate(templateName);
        var mjml = RenderLiquid(liquid, model);
        var full = RenderMjml(mjml);

        var reduced = ExtraWhitespace()
            .Replace(full, " ");

        return reduced;
    }

    private static string LoadTemplate(
        string templateName)
    {
        var assembly = typeof(LiquidTemplateLoader).Assembly;
        using var stream = assembly.GetManifestResourceStream($"EtherGizmos.Shipyard.Events.{templateName}.mjml.html")!;
        using var reader = new StreamReader(stream);
        var liquid = reader.ReadToEnd();

        return liquid;
    }

    private static string RenderLiquid(
        string liquid,
        object model)
    {
        var context = new TemplateContext
        {
            TemplateLoader = new LiquidTemplateLoader(),
        };

        var data = ScriptObject.From(model);
        context.PushGlobal(data);

        var template = Template.Parse(liquid);
        return template.Render(context);
    }

    private static string RenderMjml(
        string mjml)
    {
        var renderer = new MjmlRenderer();
        var (html, _) = renderer.Render(mjml);

        return html;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex ExtraWhitespace();
}
