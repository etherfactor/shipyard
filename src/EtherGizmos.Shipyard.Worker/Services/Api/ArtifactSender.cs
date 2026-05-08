using EtherGizmos.Shipyard.Abstractions;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace EtherGizmos.Shipyard.Services.Api;

internal class ArtifactSender : IArtifactSender
{
    private readonly ILogger _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public ArtifactSender(
        ILogger<ArtifactSender> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task SendAsync(
        int executionId,
        string contentType,
        string fileName,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        using var form = new MultipartFormDataContent();

        using var fileContent = new StreamContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        form.Add(fileContent, "file", fileName);

        using var client = _httpClientFactory.CreateClient("API");
        using var response = await client.PostAsync(
            $"/api/v1/carrierExecutions({executionId})/writeArtifact",
            form,
            cancellationToken: cancellationToken);

        response.EnsureSuccessStatusCode();

        var artifactFileName = fileName;
        var artifactContentType = contentType;
        var artifactUri = "";
        var artifactSize = 0;

        var logSize = artifactSize * 1.0m;
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

        using (_logger.BeginScope("FLAG", "ARTIFACT"))
        {
            _logger.LogInformation("Created artifact {ArtifactName} ({ArtifactContentType}) with URI {ArtifactUri}, occupying {ArtifactSize}",
                artifactFileName,
                artifactContentType,
                artifactUri,
                $"{logSize} {logSizeUnit}");
        }
    }
}
