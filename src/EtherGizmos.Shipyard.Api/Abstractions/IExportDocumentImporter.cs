using EtherGizmos.Shipyard.Api.Models;
using EtherGizmos.Shipyard.Database.Enums;

namespace EtherGizmos.Shipyard.Api.Abstractions;

public interface IExportDocumentImporter
{
    string Kind { get; }

    SecurableType SecurableType { get; }

    Task<ImporterResult> ImportAsync(
        ExportDocument document,
        CancellationToken cancellationToken = default);
}
