using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ReportControllerTests;

[TestFixture]
internal class WhenGeneratingReports
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_Data_Is_Uploaded_To_Blob_Storage_And_BlobId_Is_Saved(
        Guid reportId,
        Guid blobId,
        List<ApplicationReviewReport> entities,
        Mock<IReportRepository> repository,
        [Frozen] Mock<IBlobStorageService> blobStorageService,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.Generate(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        blobStorageService
            .Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(blobId);

        // act
        var result = await sut.Generate(repository.Object, reportId, token);

        // assert
        result.Should().BeOfType<Ok>();
        blobStorageService.Verify(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        repository.Verify(x => x.SetBlobStorageIdAsync(reportId, blobId, It.IsAny<CancellationToken>()), Times.Once());
        repository.Verify(x => x.IncrementReportDownloadCountAsync(reportId, It.IsAny<CancellationToken>()), Times.Once());
        repository.Verify(x => x.SetStatusAsync(reportId, ReportStatus.Generated, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_A_500_Is_Returned_When_An_Exception_Is_Thrown(
        Guid reportId,
        Mock<IReportRepository> repository,
        [Frozen] Mock<IBlobStorageService> blobStorageService,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.Generate(reportId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong"));

        // act
        var result = await sut.Generate(repository.Object, reportId, token);

        // assert
        result.Should().BeOfType<ProblemHttpResult>();
        (result as ProblemHttpResult)!.ProblemDetails.Status.Should().Be(500);
        repository.Verify(x => x.SetStatusAsync(reportId, ReportStatus.Failed, It.IsAny<CancellationToken>()), Times.Once());
    }
}
