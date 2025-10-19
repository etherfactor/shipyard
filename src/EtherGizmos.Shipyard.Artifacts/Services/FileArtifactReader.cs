using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Models;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.Shipyard.Services;

internal class FileArtifactReader : IArtifactReader
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public FileArtifactReader(
        IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<(ArtifactType Type, Stream Stream)> ReadAsync(
        ArtifactUri identifier,
        CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create();
        var artifactRepo = uow.Repository<Artifact>();

        var artifact = await artifactRepo.Data.SingleOrDefaultAsync(e => e.Uri == identifier.Uri, cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException($"The following artifact uri is invalid: {identifier.Uri}");

        var fs = new FileStream(artifact.PhysicalPath, FileMode.Open, FileAccess.Read);

        return (artifact.Type, fs);
    }
}
