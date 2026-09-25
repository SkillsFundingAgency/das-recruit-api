using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Report;
using SFA.DAS.Recruit.Api.Models.Responses.Report;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ReportControllerTests;

[TestFixture]
internal class WhenGeneratingReports
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_Base_Report_Data_Is_Returned(
        Guid reportId,
        List<ApplicationReviewReport> entities,
        Mock<IReportRepository> repository,
        [Frozen] Mock<IBlobStorageService> blobStorageService,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        repository
            .Setup(x => x.Generate(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await sut.Generate(repository.Object, reportId, token);

        result.Should().BeOfType<Ok<GetApplicationReviewReportResponse>>();
        var ok = (Ok<GetApplicationReviewReportResponse>)result;
        ok.Value!.ApplicationReviewReports.Should().BeEquivalentTo(entities);
        blobStorageService.Verify(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_A_500_Is_Returned_When_An_Exception_Is_Thrown(
        Guid reportId,
        Mock<IReportRepository> repository,
        [Frozen] Mock<IBlobStorageService> blobStorageService,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        repository
            .Setup(x => x.Generate(reportId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong"));

        var result = await sut.Generate(repository.Object, reportId, token);

        result.Should().BeOfType<ProblemHttpResult>();
        (result as ProblemHttpResult)!.ProblemDetails.Status.Should().Be(500);
        repository.Verify(x => x.SetStatusAsync(reportId, ReportStatus.Failed, It.IsAny<CancellationToken>()), Times.Once());
    }
}
