using EtherGizmos.Shipyard.Api.Models;

namespace EtherGizmos.Shipyard.Abstractions;

public interface IExportDocumentMigrator
{
    ExportDocument MigrateDataToCurrent(ExportDocument document);
}