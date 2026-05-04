using EtherGizmos.Shipyard.Api.Abstractions;

namespace EtherGizmos.Shipyard.Api.Services.Export;

internal class ExportDocumentImporterRegistry : IExportDocumentImporterRegistry
{
    private readonly IReadOnlyDictionary<string, IExportDocumentImporter> _importers;

    public ExportDocumentImporterRegistry(
        IEnumerable<IExportDocumentImporter> importers)
    {
        _importers = importers.ToDictionary(
            e => e.Kind,
            e => e,
            StringComparer.OrdinalIgnoreCase);
    }

    public IExportDocumentImporter? GetImporter(
        string schemaKind)
    {
        _importers.TryGetValue(schemaKind, out var importer);
        return importer;
    }
}
