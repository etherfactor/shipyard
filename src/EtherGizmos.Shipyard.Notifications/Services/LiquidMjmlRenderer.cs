using Mjml.Net;
using Scriban;
using Scriban.Runtime;

namespace EtherGizmos.Shipyard.Services;

internal static class LiquidMjmlRenderer
{
    public static string Render(
        string templateName,
        object model)
    {
        var liquid = LoadTemplate(templateName);
        var mjml = RenderLiquid(liquid, model);
        var final = RenderMjml(mjml);

        return final;
    }

    private static string LoadTemplate(
        string templateName)
    {
        var assembly = typeof(LiquidTemplateLoader).Assembly;
        using var stream = assembly.GetManifestResourceStream($"EtherGizmos.Common.Events.{templateName}.mjml.html")!;
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
}
