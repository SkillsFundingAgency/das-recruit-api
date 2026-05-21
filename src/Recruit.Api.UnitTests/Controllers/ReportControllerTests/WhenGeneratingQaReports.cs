using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Responses.Report;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ReportControllerTests;

[TestFixture]
internal class WhenGeneratingQaReports
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Qa_Report_Entities_Are_Returned(
        Guid reportId,
        List<QaReport> entities,
        Mock<IReportRepository> repository,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GenerateQa(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        repository.Setup(x => x.IncrementReportDownloadCountAsync(reportId, token));

        // act
        var result = await sut.GenerateQa(repository.Object, reportId, token);
        var payload = (result as Ok<GetQaReportResponse>)?.Value;

        // assert
        repository.Verify(x => x.GenerateQa(reportId, token), Times.Once());
        repository.Verify(x => x.IncrementReportDownloadCountAsync(reportId, token), Times.Once());
        payload.Should().NotBeNull();
        payload.QaReports.Should().BeEquivalentTo(entities, options => options.ExcludingMissingMembers());
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_An_Empty_List_Is_Returned_When_No_Report_Found(
        Guid reportId,
        Mock<IReportRepository> repository,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GenerateQa(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // act
        var result = await sut.GenerateQa(repository.Object, reportId, token);
        var payload = (result as Ok<GetQaReportResponse>)?.Value;

        // assert
        payload.Should().NotBeNull();
        payload.QaReports.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_A_500_Is_Returned_When_An_Exception_Is_Thrown(
        Guid reportId,
        Mock<IReportRepository> repository,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.GenerateQa(reportId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong"));

        // act
        var result = await sut.GenerateQa(repository.Object, reportId, token);

        // assert
        result.Should().BeOfType<ProblemHttpResult>();
        (result as ProblemHttpResult)!.ProblemDetails.Status.Should().Be(500);
    }
}
