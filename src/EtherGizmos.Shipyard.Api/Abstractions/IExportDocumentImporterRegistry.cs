namespace EtherGizmos.Shipyard.Abstractions;

public interface IExportDocumentImporterRegistry
{
    IExportDocumentImporter? GetImporter(string schemaKind);
}
