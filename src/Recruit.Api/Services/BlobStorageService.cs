using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SFA.DAS.Recruit.Api.Domain.Configuration;

namespace SFA.DAS.Recruit.Api.Services;

public interface IBlobStorageService
{
    Task<Guid> UploadAsync(string json, CancellationToken cancellationToken = default);
    Task<string> DownloadAsync(Guid blobId, CancellationToken cancellationToken = default);
}

public class BlobStorageService(BlobServiceClient blobServiceClient, BlobStorageConfiguration config) : IBlobStorageService
{
    private BlobContainerClient? _containerClient;

    public async Task<Guid> UploadAsync(string json, CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(cancellationToken);
        var blobId = Guid.NewGuid();
        var blobClient = container.GetBlobClient(blobId.ToString("N"));

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
        await blobClient.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = "application/json" }
        }, cancellationToken);

        return blobId;
    }

    public async Task<string> DownloadAsync(Guid blobId, CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(cancellationToken);
        var blobClient = container.GetBlobClient(blobId.ToString("N"));
        var response = await blobClient.DownloadContentAsync(cancellationToken);
        return response.Value.Content.ToString();
    }

    private async Task<BlobContainerClient> GetContainerAsync(CancellationToken cancellationToken)
    {
        if (_containerClient is not null)
        {
            return _containerClient;
        }
        _containerClient = blobServiceClient.GetBlobContainerClient(config.ReportsContainerName);
        await _containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        return _containerClient;
    }
}
