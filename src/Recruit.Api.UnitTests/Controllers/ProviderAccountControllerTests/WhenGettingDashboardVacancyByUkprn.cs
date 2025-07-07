using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ProviderAccountControllerTests;
[TestFixture]
public class WhenGettingDashboardVacancyByUkprn
{
    [Test, MoqAutoData]
    public async Task Then_The_Count_ReturnsOk(
        int ukprn,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        List<ApplicationReviewStatus> status,
        PaginatedList<VacancyDetail> mockVacancies,
        VacancyDashboardResponse mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ProviderAccountController controller,
        CancellationToken token)
    {
        // Arrange
        providerMock.Setup(p => p.GetPagedByUkprnAndStatusAsync(ukprn, pageNumber, pageSize, sortColumn, isAscending, status, token))
            .ReturnsAsync(mockVacancies);

        // Act
        var result = await controller.GetDashboardVacanciesCountByAccountId(ukprn, pageNumber, pageSize, sortColumn, isAscending, status, token);

        // Assert
        result.Should().BeOfType<Ok<VacancyDashboardResponse>>();
        var okResult = result as Ok<VacancyDashboardResponse>;

        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value!.Items.Should().BeEquivalentTo(mockVacancies.Items);
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_Exception(
        int ukprn,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        List<ApplicationReviewStatus> status,
        PaginatedList<VacancyDetail> mockVacancies,
        VacancyDashboardResponse mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ProviderAccountController controller,
        CancellationToken token)
    {
        // Arrange
        // Arrange
        providerMock.Setup(p => p.GetPagedByUkprnAndStatusAsync(ukprn, pageNumber, pageSize, sortColumn, isAscending, status, token))
            .ThrowsAsync(new Exception());

        // Act
        var result = await controller.GetDashboardVacanciesCountByAccountId(ukprn, pageNumber, pageSize, sortColumn, isAscending, status, token);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}
