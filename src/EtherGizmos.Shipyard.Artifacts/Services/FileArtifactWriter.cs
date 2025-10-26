using EtherGizmos.Common.Extensions;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO.Compression;
using System.Transactions;

namespace EtherGizmos.Shipyard.Services;

internal class FileArtifactWriter : IArtifactWriter
{
    private readonly ILogger _logger;
    private readonly IOptionsMonitor<ArtifactOptions> _options;
    private readonly IUnitOfWorkFactory _uowFactory;

    public FileArtifactWriter(
        ILogger<FileArtifactWriter> logger,
        IOptionsMonitor<ArtifactOptions> options,
        IUnitOfWorkFactory uowFactory)
    {
        _logger = logger;
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

        var useFileName = fileName;

        if (!useFileName.EndsWith(type.Extension))
            useFileName += $".{type.Extension}";

        var recordFileName = useFileName;

        if (type.ShouldGzip)
            useFileName += ".gz";

        useFileName = $"{id}-{useFileName}";

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
            FileName = recordFileName,
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

        var logSize = artifact.Bytes * 1.0m;
        var logSizeUnit = "B";

        if (logSize >= 1024)
        {
            logSize /= 1024;
            logSizeUnit = "KB";
        }

        if (logSize >= 1024)
        {
            logSize /= 1024;
            logSizeUnit = "MB";
        }

        if (logSize >= 1024)
        {
            logSize /= 1024;
            logSizeUnit = "GB";
        }

        logSize = Math.Round(logSize, 1);

        using (_logger.BeginKeyedScope("FLAG", "ARTIFACT"))
            _logger.LogInformation("Created artifact {ArtifactName} ({ArtifactContentType}) with URI {ArtifactUri}, occupying {ArtifactSize}",
                recordFileName,
                artifact.ContentType,
                artifact.Uri.ToString(),
                $"{logSize} {logSizeUnit}");

        return new(new(artifact.Uri), artifact.ContentType, recordFileName, artifact.Bytes);
    }
}
