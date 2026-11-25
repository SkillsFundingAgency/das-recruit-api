using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ReportControllerTests;

public class WhenCleaningUp
{
    [Test, MoqAutoData]
    public async Task Then_The_Call_Is_Made_To_Clean_Up_The_Database_Records(
        Mock<IReportRepository> reportRepository,
        [Greedy] ReportController sut)
    {
        // act
        var result = await sut.Cleanup(reportRepository.Object, CancellationToken.None) as Ok;

        // assert
        result.Should().NotBeNull();
        reportRepository.Verify(r => r.DeleteManyByAgeAsync(
            It.Is<ReportOwnerType>(x => x == ReportOwnerType.Provider),
            It.Is<ReportType>(x => x == ReportType.ProviderApplications),
            It.Is<int>(x => x == 7),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}