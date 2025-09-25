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
            string userId,
            VacancyDashboardModel mockVacancyDashboardModelResponse,
            ApplicationReviewsDashboardModel mockApplicationReviewsDashboardModelResponse,
            EmployerTransferredVacanciesAlertModel mockEmployerTransferredVacanciesAlertModelResponse,
            EmployerTransferredVacanciesAlertModel mockBlockedProviderTransferredVacanciesAlertModelResponse,
            BlockedProviderAlertModel mockBlockedProviderAlertModelResponse,
            WithdrawnVacanciesAlertModel mockWithdrawnByQaAlertModelResponse,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Frozen] Mock<IVacancyProvider> vacancyMock,
            [Frozen] Mock<IAlertsProvider> alertsMock,
            [Greedy] EmployerAccountController controller,
            CancellationToken token)
        {
            // Arrange
            providerMock.Setup(p => p.GetCountByAccountId(accountId, token))
                .ReturnsAsync(mockApplicationReviewsDashboardModelResponse);
            vacancyMock.Setup(v => v.GetCountByAccountId(accountId, token)).ReturnsAsync(mockVacancyDashboardModelResponse);
            alertsMock.Setup(a => a.GetEmployerTransferredVacanciesAlertByAccountId(accountId, userId, TransferReason.EmployerRevokedPermission, token))
                .ReturnsAsync(mockEmployerTransferredVacanciesAlertModelResponse);
            alertsMock.Setup(a => a.GetEmployerTransferredVacanciesAlertByAccountId(accountId, userId, TransferReason.BlockedByQa, token))
                .ReturnsAsync(mockBlockedProviderTransferredVacanciesAlertModelResponse);
            alertsMock.Setup(a => a.GetBlockedProviderAlertCountByAccountId(accountId, userId, token))
                .ReturnsAsync(mockBlockedProviderAlertModelResponse);
            alertsMock.Setup(a => a.GetWithDrawnByQaAlertByAccountId(accountId, userId, token))
                .ReturnsAsync(mockWithdrawnByQaAlertModelResponse);

            // Act
            var result = await controller.GetDashboardCountByAccountId(accountId, userId, token);

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

            okResult.Value.EmployerRevokedTransferredVacanciesAlert.Should()
                .Be(mockEmployerTransferredVacanciesAlertModelResponse);
            okResult.Value.BlockedProviderTransferredVacanciesAlert.Should()
                .Be(mockBlockedProviderTransferredVacanciesAlertModelResponse);
            okResult.Value.BlockedProviderAlert.Should().Be(mockBlockedProviderAlertModelResponse);
            okResult.Value.WithDrawnByQaVacanciesAlert.Should().Be(mockWithdrawnByQaAlertModelResponse);
        }

        [Test, MoqAutoData]
        public async Task Then_UserId_Is_Null_The_Count_ReturnsOk(
            long accountId,
            VacancyDashboardModel mockVacancyDashboardModelResponse,
            ApplicationReviewsDashboardModel mockApplicationReviewsDashboardModelResponse,
            EmployerTransferredVacanciesAlertModel mockEmployerTransferredVacanciesAlertModelResponse,
            EmployerTransferredVacanciesAlertModel mockBlockedProviderTransferredVacanciesAlertModelResponse,
            BlockedProviderAlertModel mockBlockedProviderAlertModelResponse,
            WithdrawnVacanciesAlertModel mockWithdrawnByQaAlertModelResponse,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Frozen] Mock<IVacancyProvider> vacancyMock,
            [Frozen] Mock<IAlertsProvider> alertsMock,
            [Greedy] EmployerAccountController controller,
            CancellationToken token)
        {
            // Arrange
            providerMock.Setup(p => p.GetCountByAccountId(accountId, token))
                .ReturnsAsync(mockApplicationReviewsDashboardModelResponse);
            vacancyMock.Setup(v => v.GetCountByAccountId(accountId, token)).ReturnsAsync(mockVacancyDashboardModelResponse);
            

            // Act
            var result = await controller.GetDashboardCountByAccountId(accountId, null, token);

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

            alertsMock.Verify(
                a => a.GetEmployerTransferredVacanciesAlertByAccountId(accountId, null,
                    TransferReason.EmployerRevokedPermission, token), Times.Never);
            alertsMock.Verify(a =>
                a.GetEmployerTransferredVacanciesAlertByAccountId(accountId, null, TransferReason.BlockedByQa, token), Times.Never);
            alertsMock.Verify(a => a.GetBlockedProviderAlertCountByAccountId(accountId, null, token), Times.Never());
            alertsMock.Verify(a => a.GetWithDrawnByQaAlertByAccountId(accountId, null, token), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Exception(
            long accountId,
            string userId,
            ApplicationReviewStatus status,
            ApplicationReviewsDashboardModel mockResponse,
            CancellationToken token,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Greedy] EmployerAccountController controller)
        {
            // Arrange
            // Arrange
            providerMock.Setup(p => p.GetCountByAccountId(accountId, token))
                .ThrowsAsync(new Exception());

            // Act
            var result = await controller.GetDashboardCountByAccountId(accountId, userId, token);

            // Assert
            result.Should().BeOfType<ProblemHttpResult>();
        }
    }
}
