using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.EmployerAccountControllerTests
{
    [TestFixture]
    public class WhenGettingDashboardCountByAccountId
    {
        [Test, MoqAutoData]
        public async Task Then_The_Count_ReturnsOk(
            long accountId,
            VacancyDashboardModel mockVacancyDashboardModelResponse,
            ApplicationReviewsDashboardModel mockApplicationReviewsDashboardModelResponse,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Frozen] Mock<IVacancyProvider> vacancyMock,
            [Greedy] EmployerAccountController controller,
            CancellationToken token)
        {
            // Arrange
            providerMock.Setup(p => p.GetCountByAccountId(accountId, token))
                .ReturnsAsync(mockApplicationReviewsDashboardModelResponse);
            vacancyMock.Setup(v => v.GetCountByAccountId(accountId, token)).ReturnsAsync(mockVacancyDashboardModelResponse);
            

            // Act
            var result = await controller.GetDashboardCountByAccountId(accountId, token);

            // Assert
            result.Should().BeOfType<Ok<EmployerDashboardModel>>();
            var okResult = result as Ok<EmployerDashboardModel>;

            okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value.NewApplicationsCount!.Should().Be(mockApplicationReviewsDashboardModelResponse.NewApplicationsCount);
            okResult.Value.SharedApplicationsCount!.Should().Be(mockApplicationReviewsDashboardModelResponse.SharedApplicationsCount);
            okResult.Value.AllSharedApplicationsCount!.Should().Be(mockApplicationReviewsDashboardModelResponse.AllSharedApplicationsCount);
            okResult.Value.UnsuccessfulApplicationsCount!.Should().Be(mockApplicationReviewsDashboardModelResponse.UnsuccessfulApplicationsCount);
            okResult.Value.EmployerReviewedApplicationsCount!.Should().Be(mockApplicationReviewsDashboardModelResponse.EmployerReviewedApplicationsCount);
            okResult.Value.HasNoApplications!.Should().Be(mockApplicationReviewsDashboardModelResponse.HasNoApplications);

            okResult.Value.ClosedVacanciesCount!.Should().Be(mockVacancyDashboardModelResponse.ClosedVacanciesCount);
            okResult.Value.DraftVacanciesCount!.Should().Be(mockVacancyDashboardModelResponse.DraftVacanciesCount);
            okResult.Value.LiveVacanciesCount!.Should().Be(mockVacancyDashboardModelResponse.LiveVacanciesCount);
            okResult.Value.ReviewVacanciesCount!.Should().Be(mockVacancyDashboardModelResponse.ReviewVacanciesCount);
            okResult.Value.ReferredVacanciesCount!.Should().Be(mockVacancyDashboardModelResponse.ReferredVacanciesCount);
            okResult.Value.SubmittedVacanciesCount!.Should().Be(mockVacancyDashboardModelResponse.SubmittedVacanciesCount);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Exception(
            long accountId,
            ApplicationReviewStatus status,
            ApplicationReviewsDashboardModel mockResponse,
            CancellationToken token,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Greedy] EmployerAccountController controller)
        {
            // Arrange
            providerMock.Setup(p => p.GetCountByAccountId(accountId, token))
                .ThrowsAsync(new Exception());

            // Act
            var result = await controller.GetDashboardCountByAccountId(accountId, token);

            // Assert
            result.Should().BeOfType<ProblemHttpResult>();
        }
    }
}
