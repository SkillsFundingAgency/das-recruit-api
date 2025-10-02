using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ProviderAccountControllerTests;
[TestFixture]
internal class WhenGettingProviderAlertsByUkprn
{
    [Test, MoqAutoData]
    public async Task Then_The_Model_ReturnsOk(
            int ukprn,
            string userId,
            ProviderTransferredVacanciesAlertModel mockProviderTransferredVacanciesAlertModelResponse,
            WithdrawnVacanciesAlertModel mockWithdrawnVacanciesAlertModelResponse,
            [Frozen] Mock<IAlertsProvider> alertsMock,
            [Greedy] ProviderAccountController controller,
            CancellationToken token)
    {
        // Arrange
        alertsMock.Setup(a => a.GetProviderTransferredVacanciesAlertByUkprn(ukprn, userId, token))
            .ReturnsAsync(mockProviderTransferredVacanciesAlertModelResponse);
        alertsMock.Setup(a => a.GetWithDrawnByQaAlertByUkprnId(ukprn, userId, token))
            .ReturnsAsync(mockWithdrawnVacanciesAlertModelResponse);

        // Act
        var result = await controller.GetProviderAlertsByAccountId(ukprn, userId, token);

        // Assert
        result.Should().BeOfType<Ok<ProviderAlertsModel>>();
        var okResult = result as Ok<ProviderAlertsModel>;

        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);

        okResult.Value.ProviderTransferredVacanciesAlert!.Should().BeEquivalentTo(mockProviderTransferredVacanciesAlertModelResponse);
        okResult.Value.WithdrawnVacanciesAlert!.Should().BeEquivalentTo(mockWithdrawnVacanciesAlertModelResponse);
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_Exception(
        int ukprn,
        string userId,
        [Frozen] Mock<IAlertsProvider> alertsMock,
        [Greedy] ProviderAccountController controller,
        CancellationToken token)
    {
        // Arrange
        alertsMock.Setup(a => a.GetProviderTransferredVacanciesAlertByUkprn(ukprn, userId, token))
            .ThrowsAsync(new Exception());

        // Act
        var result = await controller.GetProviderAlertsByAccountId(ukprn, userId, token);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}