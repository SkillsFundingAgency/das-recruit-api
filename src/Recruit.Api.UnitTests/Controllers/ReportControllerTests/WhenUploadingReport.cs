using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Report;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ReportControllerTests;

[TestFixture]
internal class WhenUploadingReport
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_Enriched_Data_Is_Uploaded_To_Blob_Storage_And_BlobId_Is_Saved(
        Guid reportId,
        Guid blobId,
        List<ApplicationSummaryReport> reports,
        Mock<IReportRepository> repository,
        [Frozen] Mock<IBlobStorageService> blobStorageService,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        var request = new PostUploadApplicationSummaryReportRequest { Reports = reports };
        blobStorageService
            .Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(blobId);

        var result = await sut.Upload(repository.Object, reportId, request, token);

        result.Should().BeOfType<Ok>();
        blobStorageService.Verify(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        repository.Verify(x => x.SetBlobStorageIdAsync(reportId, blobId, It.IsAny<CancellationToken>()), Times.Once());
        repository.Verify(x => x.SetStatusAsync(reportId, ReportStatus.Generated, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_Empty_Reports_Still_Uploads_Blob_And_Marks_As_Generated(
        Guid reportId,
        Guid blobId,
        Mock<IReportRepository> repository,
        [Frozen] Mock<IBlobStorageService> blobStorageService,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        var request = new PostUploadApplicationSummaryReportRequest { Reports = [] };
        blobStorageService
            .Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(blobId);

        var result = await sut.Upload(repository.Object, reportId, request, token);

        result.Should().BeOfType<Ok>();
        blobStorageService.Verify(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        repository.Verify(x => x.SetBlobStorageIdAsync(reportId, blobId, It.IsAny<CancellationToken>()), Times.Once());
        repository.Verify(x => x.SetStatusAsync(reportId, ReportStatus.Generated, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_A_500_Is_Returned_When_An_Exception_Is_Thrown(
        Guid reportId,
        List<ApplicationSummaryReport> reports,
        Mock<IReportRepository> repository,
        [Frozen] Mock<IBlobStorageService> blobStorageService,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        var request = new PostUploadApplicationSummaryReportRequest { Reports = reports };
        blobStorageService
            .Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Blob upload failed"));

        var result = await sut.Upload(repository.Object, reportId, request, token);

        result.Should().BeOfType<ProblemHttpResult>();
        (result as ProblemHttpResult)!.ProblemDetails.Status.Should().Be(500);
        repository.Verify(x => x.SetStatusAsync(reportId, ReportStatus.Failed, It.IsAny<CancellationToken>()), Times.Once());
    }
}
