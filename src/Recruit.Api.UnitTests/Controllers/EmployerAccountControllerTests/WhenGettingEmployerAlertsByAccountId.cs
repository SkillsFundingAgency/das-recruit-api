using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.EmployerAccountControllerTests;
[TestFixture]
internal class WhenGettingEmployerAlertsByAccountId
{
    [Test, MoqAutoData]
    public async Task Then_The_Model_ReturnsOk(
            long accountId,
            string userId,
            EmployerTransferredVacanciesAlertModel mockEmployerTransferredVacanciesAlertModelResponse,
            EmployerTransferredVacanciesAlertModel mockBlockedProviderTransferredVacanciesAlertModelResponse,
            BlockedProviderAlertModel mockBlockedProviderAlertModelResponse,
            WithdrawnVacanciesAlertModel mockWithdrawnByQaAlertModelResponse,
            [Frozen] Mock<IAlertsProvider> alertsMock,
            [Greedy] EmployerAccountController controller,
            CancellationToken token)
    {
        // Arrange
        alertsMock.Setup(a => a.GetEmployerTransferredVacanciesAlertByAccountId(accountId, userId, TransferReason.EmployerRevokedPermission, token))
            .ReturnsAsync(mockEmployerTransferredVacanciesAlertModelResponse);
        alertsMock.Setup(a => a.GetEmployerTransferredVacanciesAlertByAccountId(accountId, userId, TransferReason.BlockedByQa, token))
            .ReturnsAsync(mockBlockedProviderTransferredVacanciesAlertModelResponse);
        alertsMock.Setup(a => a.GetBlockedProviderAlertCountByAccountId(accountId, userId, token))
            .ReturnsAsync(mockBlockedProviderAlertModelResponse);
        alertsMock.Setup(a => a.GetWithDrawnByQaAlertByAccountId(accountId, userId, token))
            .ReturnsAsync(mockWithdrawnByQaAlertModelResponse);

        // Act
        var result = await controller.GetEmployerAlertsByAccountId(accountId, userId, token);

        // Assert
        result.Should().BeOfType<Ok<EmployerAlertsModel>>();
        var okResult = result as Ok<EmployerAlertsModel>;

        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        
        okResult.Value.EmployerRevokedTransferredVacanciesAlert.Should()
            .Be(mockEmployerTransferredVacanciesAlertModelResponse);
        okResult.Value.BlockedProviderTransferredVacanciesAlert.Should()
            .Be(mockBlockedProviderTransferredVacanciesAlertModelResponse);
        okResult.Value.BlockedProviderAlert.Should().Be(mockBlockedProviderAlertModelResponse);
        okResult.Value.WithDrawnByQaVacanciesAlert.Should().Be(mockWithdrawnByQaAlertModelResponse);
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_Exception(
        long accountId,
        string userId,
        CancellationToken token,
        [Frozen] Mock<IAlertsProvider> alertsMock,
        [Greedy] EmployerAccountController controller)
    {
        // Arrange
        alertsMock.Setup(a => a.GetEmployerTransferredVacanciesAlertByAccountId(accountId, userId, TransferReason.EmployerRevokedPermission, token))
            .ThrowsAsync(new Exception());

        // Act
        var result = await controller.GetEmployerAlertsByAccountId(accountId, userId, token);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}
