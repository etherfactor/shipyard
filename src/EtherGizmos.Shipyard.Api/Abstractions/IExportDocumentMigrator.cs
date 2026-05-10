using EtherGizmos.Shipyard.Api.Models;

namespace EtherGizmos.Shipyard.Api.Abstractions;

public interface IExportDocumentMigrator
{
    ExportDocument MigrateDataToCurrent(ExportDocument document);
}