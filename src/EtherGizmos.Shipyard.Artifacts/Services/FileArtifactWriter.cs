using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Models;
using Microsoft.Extensions.Options;
using System.IO.Compression;
using System.Transactions;

namespace EtherGizmos.Shipyard.Services;

internal class FileArtifactWriter : IArtifactWriter
{
    private readonly IOptionsMonitor<ArtifactOptions> _options;
    private readonly IUnitOfWorkFactory _uowFactory;

    public FileArtifactWriter(
        IOptionsMonitor<ArtifactOptions> options,
        IUnitOfWorkFactory uowFactory)
    {
        _options = options;
        _uowFactory = uowFactory;
    }

    public async Task<ArtifactDescriptor> WriteAsync(
        string container,
        ArtifactFormat type,
        string fileName,
        Stream data,
        CancellationToken cancellationToken = default)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var id = Guid.NewGuid();

        var useFileName = $"{id}-{fileName}";

        if (!useFileName.EndsWith(type.Extension))
            useFileName += $".{type.Extension}";

        if (type.ShouldGzip)
            useFileName += ".gz";

        var basePath = _options.CurrentValue.BasePath;
        var fullPath = Path.GetFullPath(Path.Combine(basePath, container, useFileName));

        var directory = Path.GetDirectoryName(fullPath)!;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var artifact = new Artifact()
        {
            Uri = $"artifact://{container}/{id}",
            ContentType = type.ContentType,
            Bytes = 0,
            FileName = fileName,
            PhysicalPath = fullPath,
        };

        using var uow = _uowFactory.Create();
        var artifactRepo = uow.Repository<Artifact>();

        artifactRepo.Create(artifact);

        await uow.SaveChangesAsync(cancellationToken);

        using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        var writeToTmp = fs as Stream;

        if (type.ShouldGzip)
        {
            var gz = new GZipStream(fs, CompressionLevel.SmallestSize);
            writeToTmp = gz;
        }

        using var writeTo = writeToTmp;

        await data.CopyToAsync(writeTo, cancellationToken);

        writeTo.Dispose();

        var info = new FileInfo(fullPath);
        artifact.Bytes = info.Length;

        await uow.SaveChangesAsync(cancellationToken);

        scope.Complete();

        return new(new(artifact.Uri), artifact.ContentType, artifact.Bytes);
    }
}
