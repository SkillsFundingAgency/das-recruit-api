using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.EmployerAccountControllerTests;
[TestFixture]
public class WhenGettingDashboardVacanciesCountByAccountId
{
    [Test, MoqAutoData]
    public async Task Then_The_Count_ReturnsOk(
        long accountId,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        ApplicationReviewStatus status,
        PaginatedList<VacancyDetail> mockVacancies,
        VacancyDashboardResponse mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] EmployerAccountController controller,
        CancellationToken token)
    {
        // Arrange
        providerMock.Setup(p => p.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, new List<ApplicationReviewStatus>{status} , token))
            .ReturnsAsync(mockVacancies);

        // Act
        var result = await controller.GetDashboardVacanciesCountByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, [status], token);

        // Assert
        result.Should().BeOfType<Ok<VacancyDashboardResponse>>();
        var okResult = result as Ok<VacancyDashboardResponse>;

        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value!.Items.Should().BeEquivalentTo(mockVacancies.Items);
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_Exception(
        long accountId,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        ApplicationReviewStatus status,
        PaginatedList<VacancyDetail> mockVacancies,
        VacancyDashboardResponse mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] EmployerAccountController controller,
        CancellationToken token)
    {
        // Arrange
        // Arrange
        providerMock.Setup(p => p.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, new List<ApplicationReviewStatus>{status}, token))
            .ThrowsAsync(new Exception());

        // Act
        var result = await controller.GetDashboardVacanciesCountByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, [status], token);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}
