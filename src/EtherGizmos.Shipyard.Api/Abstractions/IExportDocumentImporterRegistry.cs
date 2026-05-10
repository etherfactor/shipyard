namespace EtherGizmos.Shipyard.Api.Abstractions;

public interface IExportDocumentImporterRegistry
{
    IExportDocumentImporter? GetImporter(string schemaKind);
}
