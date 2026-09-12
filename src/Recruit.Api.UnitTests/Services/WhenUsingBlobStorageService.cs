using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.UnitTests.Services;

public class WhenUsingBlobStorageService
{
    [Test, MoqAutoData]
    public async Task UploadAsync_ReturnsNonEmptyGuid(
        BlobStorageConfiguration config,
        Mock<BlobClient> blobClient,
        Mock<BlobContainerClient> containerClient,
        Mock<BlobServiceClient> blobServiceClient)
    {
        SetupBlobContainer(blobServiceClient, containerClient, blobClient);
        blobClient
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());
        var sut = new BlobStorageService(blobServiceClient.Object, config);

        var result = await sut.UploadAsync("{\"test\": true}");

        result.Should().NotBeEmpty();
    }

    [Test, MoqAutoData]
    public async Task UploadAsync_SetsContentTypeToJson(
        BlobStorageConfiguration config,
        Mock<BlobClient> blobClient,
        Mock<BlobContainerClient> containerClient,
        Mock<BlobServiceClient> blobServiceClient)
    {
        SetupBlobContainer(blobServiceClient, containerClient, blobClient);
        blobClient
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());
        var sut = new BlobStorageService(blobServiceClient.Object, config);

        await sut.UploadAsync("{\"test\": true}");

        blobClient.Verify(x => x.UploadAsync(
            It.IsAny<Stream>(),
            It.Is<BlobUploadOptions>(o => o.HttpHeaders!.ContentType == "application/json"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task UploadAsync_UsesBlobNameDerivedFromReturnedGuid(
        BlobStorageConfiguration config,
        Mock<BlobClient> blobClient,
        Mock<BlobContainerClient> containerClient,
        Mock<BlobServiceClient> blobServiceClient)
    {
        string? capturedBlobName = null;
        SetupBlobContainer(blobServiceClient, containerClient, blobClient);
        containerClient
            .Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Callback((string name) => capturedBlobName = name)
            .Returns(blobClient.Object);
        blobClient
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());
        var sut = new BlobStorageService(blobServiceClient.Object, config);

        var blobId = await sut.UploadAsync("{}");

        capturedBlobName.Should().Be(blobId.ToString("N"));
    }

    [Test, MoqAutoData]
    public async Task DownloadAsync_ReturnsContent(
        string expectedContent,
        BlobStorageConfiguration config,
        Mock<BlobClient> blobClient,
        Mock<BlobContainerClient> containerClient,
        Mock<BlobServiceClient> blobServiceClient)
    {
        SetupBlobContainer(blobServiceClient, containerClient, blobClient);
        var downloadResult = BlobsModelFactory.BlobDownloadResult(content: BinaryData.FromString(expectedContent));
        blobClient
            .Setup(x => x.DownloadContentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(downloadResult, Mock.Of<Response>()));
        var sut = new BlobStorageService(blobServiceClient.Object, config);

        var result = await sut.DownloadAsync(Guid.NewGuid());

        result.Should().Be(expectedContent);
    }

    [Test, MoqAutoData]
    public async Task DownloadAsync_UsesGuidAsBlobName(
        Guid blobId,
        BlobStorageConfiguration config,
        Mock<BlobClient> blobClient,
        Mock<BlobContainerClient> containerClient,
        Mock<BlobServiceClient> blobServiceClient)
    {
        SetupBlobContainer(blobServiceClient, containerClient, blobClient);
        var downloadResult = BlobsModelFactory.BlobDownloadResult(content: BinaryData.FromString("{}"));
        blobClient
            .Setup(x => x.DownloadContentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(downloadResult, Mock.Of<Response>()));
        var sut = new BlobStorageService(blobServiceClient.Object, config);

        await sut.DownloadAsync(blobId);

        containerClient.Verify(x => x.GetBlobClient(blobId.ToString("N")), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task ContainerClient_IsCreatedOnce_AcrossMultipleCalls(
        BlobStorageConfiguration config,
        Mock<BlobClient> blobClient,
        Mock<BlobContainerClient> containerClient,
        Mock<BlobServiceClient> blobServiceClient)
    {
        SetupBlobContainer(blobServiceClient, containerClient, blobClient);
        blobClient
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());
        var sut = new BlobStorageService(blobServiceClient.Object, config);

        await sut.UploadAsync("{}");
        await sut.UploadAsync("{}");

        blobServiceClient.Verify(x => x.GetBlobContainerClient(It.IsAny<string>()), Times.Once);
        containerClient.Verify(x => x.CreateIfNotExistsAsync(
            It.IsAny<PublicAccessType>(),
            It.IsAny<IDictionary<string, string>>(),
            It.IsAny<BlobContainerEncryptionScopeOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private static void SetupBlobContainer(
        Mock<BlobServiceClient> blobServiceClient,
        Mock<BlobContainerClient> containerClient,
        Mock<BlobClient> blobClient)
    {
        blobServiceClient
            .Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(containerClient.Object);
        containerClient
            .Setup(x => x.CreateIfNotExistsAsync(
                It.IsAny<PublicAccessType>(),
                It.IsAny<IDictionary<string, string>>(),
                It.IsAny<BlobContainerEncryptionScopeOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContainerInfo>>());
        containerClient
            .Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClient.Object);
    }
}
