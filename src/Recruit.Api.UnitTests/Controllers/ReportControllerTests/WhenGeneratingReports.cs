using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Responses.Report;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ReportControllerTests;
[TestFixture]
internal class WhenGeneratingReports
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Vacancy_Entities_Are_Returned(
        Guid reportId,
        List<ApplicationReviewReport> entities,
        Mock<IReportRepository> repository,
        [Greedy] ReportController sut,
        CancellationToken token)
    {
        // arrange
        repository
            .Setup(x => x.Generate(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        // act
        var result = await sut.Generate(repository.Object, reportId, token);
        var payload = (result as Ok<GetApplicationReviewReportResponse>)?.Value;

        // assert
        repository.Verify(x => x.Generate(reportId, token), Times.Once());
        payload.Should().NotBeNull();
        payload.ApplicationReviewReports.Should().BeEquivalentTo(entities, options => options.ExcludingMissingMembers());
    }
}