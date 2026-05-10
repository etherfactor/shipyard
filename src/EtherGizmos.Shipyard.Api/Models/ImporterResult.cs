namespace EtherGizmos.Shipyard.Api.Models;

public record ImporterResult(
    string Kind,
    int SchemaVersion,
    object? Id,
    object? Identifier,
    ImporterResultStatusType Status,
    string? ErrorMessage = null);
