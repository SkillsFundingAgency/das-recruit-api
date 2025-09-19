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
            ApplicationReviewStatus status,
            DashboardModel mockResponse,
            CancellationToken token,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Greedy] EmployerAccountController controller)
        {
            // Arrange
            providerMock.Setup(p => p.GetCountByAccountId(accountId, token))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await controller.GetDashboardCountByAccountId(accountId, token);

            // Assert
            result.Should().BeOfType<Ok<DashboardModel>>();
            var okResult = result as Ok<DashboardModel>;

            okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value!.Should().BeEquivalentTo(mockResponse);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Exception(
            long accountId,
            ApplicationReviewStatus status,
            DashboardModel mockResponse,
            CancellationToken token,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Greedy] EmployerAccountController controller)
        {
            // Arrange
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
