using EtherGizmos.Shipyard.Api.Models;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Models;

namespace EtherGizmos.Shipyard.Abstractions;

public interface IExportDocumentImporter
{
    string Kind { get; }

    SecurableType SecurableType { get; }

    Task<ImporterResult> ImportAsync(
        ExportDocument document,
        CancellationToken cancellationToken = default);

    Task<ImporterResult> VerifyAsync(
        ExportDocument document,
        CancellationToken cancellationToken = default);
}
