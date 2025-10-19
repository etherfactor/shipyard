using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Models;
using Microsoft.Extensions.Options;
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

    public async Task<ArtifactUri> WriteAsync(
        string container,
        ArtifactType type,
        string fileName,
        Stream data,
        CancellationToken cancellationToken = default)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var id = Guid.NewGuid();

        var basePath = _options.CurrentValue.BasePath;
        var fullPath = Path.GetFullPath(Path.Combine(basePath, container, $"{id}-{fileName}.{type.ToExtension()}"));

        var directory = Path.GetDirectoryName(fullPath)!;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var artifact = new Artifact()
        {
            Uri = $"artifact://{container}/{id}",
            Type = type,
            Bytes = data.Length,
            FileName = fileName,
            PhysicalPath = fullPath,
        };

        using var uow = _uowFactory.Create();
        var artifactRepo = uow.Repository<Artifact>();

        artifactRepo.Create(artifact);

        await uow.SaveChangesAsync(cancellationToken);

        using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await data.CopyToAsync(fs, cancellationToken);

        scope.Complete();

        return new(artifact.Uri);
    }
}
