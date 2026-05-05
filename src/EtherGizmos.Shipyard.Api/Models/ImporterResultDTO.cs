using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.Shipyard.Models;

public class ImporterResultDTO
{
    public string Kind { get; set; } = null!;

    public int SchemaVersion { get; set; }

    public object? Id { get; set; }

    public object? Identifier { get; set; }

    public ImporterResultStatusType Status { get; set; }

    public string? ErrorMessage { get; set; }
}

[ExcludeFromCodeCoverage]
public static class ImporterResultDTOExamples
{
    public static ImporterResultDTO Get { get; } = new()
    {
        Kind = "carrier",
        SchemaVersion = 1,
        Id = 1,
        Identifier = "usps",
        Status = ImporterResultStatusType.Updated,
        ErrorMessage = null,
    };
}

[ExcludeFromCodeCoverage]
public class ImporterResultDTOExampleGet : IExamplesProvider<ImporterResultDTO>
{
    public ImporterResultDTO GetExamples()
    {
        return ImporterResultDTOExamples.Get;
    }
}
