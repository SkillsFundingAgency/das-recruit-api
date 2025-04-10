using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace Recruit.Api.UnitTests.Controllers.EmployerAccountControllerTests
{
    [TestFixture]
    public class WhenGettingDashboardCountByAccountId
    {
        [Test, MoqAutoData]
        public async Task Then_The_Count_ReturnsOk(
            long accountId,
            ApplicationStatus status,
            DashboardModel mockResponse,
            CancellationToken token,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Greedy] EmployerAccountController controller)
        {
            // Arrange
            providerMock.Setup(p => p.GetCountByAccountId(accountId, status , token))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await controller.GetDashboardCountByAccountId(accountId, status, token);

            // Assert
            result.Should().BeOfType<Ok<DashboardModel>>();
            var okResult = result as Ok<DashboardModel>;

            okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value!.Should().BeEquivalentTo(mockResponse);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Exception(
            long accountId,
            ApplicationStatus status,
            DashboardModel mockResponse,
            CancellationToken token,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Greedy] EmployerAccountController controller)
        {
            // Arrange
            // Arrange
            providerMock.Setup(p => p.GetCountByAccountId(accountId, status, token))
                .ThrowsAsync(new Exception());

            // Act
            var result = await controller.GetDashboardCountByAccountId(accountId, status, token);

            // Assert
            result.Should().BeOfType<ProblemHttpResult>();
        }
    }
}
