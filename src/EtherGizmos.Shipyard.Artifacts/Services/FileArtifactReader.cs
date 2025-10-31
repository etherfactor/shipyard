using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Models;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace EtherGizmos.Shipyard.Services;

internal class FileArtifactReader : IArtifactReader
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public FileArtifactReader(
        IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<ArtifactRead> ReadAsync(
        ArtifactUri identifier,
        CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create();
        var artifactRepo = uow.Repository<Artifact>();

        var artifact = await artifactRepo.Data.SingleOrDefaultAsync(e => e.Uri == identifier.Value, cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException($"The following artifact uri is invalid: {identifier.Value}");

        var fs = new FileStream(artifact.PhysicalPath, FileMode.Open, FileAccess.Read);
        var readFrom = fs as Stream;

        if (artifact.PhysicalPath.EndsWith(".gz"))
        {
            var gz = new GZipStream(fs, CompressionMode.Decompress);
            readFrom = gz;
        }

        return new(identifier, artifact.FileName, artifact.ContentType, artifact.Bytes, readFrom);
    }
}
