using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ProviderAccountControllerTests
{
    [TestFixture]
    public class WhenGettingDashboardCountByUkprn
    {
        [Test, MoqAutoData]
        public async Task Then_The_Count_ReturnsOk(
            int ukprn,
            ApplicationReviewStatus status,
            DashboardModel mockResponse,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Greedy] ProviderAccountController controller,
            CancellationToken token)
        {
            // Arrange
            providerMock.Setup(p => p.GetCountByUkprn(ukprn, status , token))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await controller.GetDashboardCountByUkprn(ukprn, status, token);

            // Assert
            result.Should().BeOfType<Ok<DashboardModel>>();
            var okResult = result as Ok<DashboardModel>;

            okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value!.Should().BeEquivalentTo(mockResponse);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Exception(
            int ukprn,
            ApplicationReviewStatus status,
            DashboardModel mockResponse,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Greedy] ProviderAccountController controller,
            CancellationToken token)
        {
            // Arrange
            // Arrange
            providerMock.Setup(p => p.GetCountByUkprn(ukprn, status, token))
                .ThrowsAsync(new Exception());

            // Act
            var result = await controller.GetDashboardCountByUkprn(ukprn, status, token);

            // Assert
            result.Should().BeOfType<ProblemHttpResult>();
        }
    }
}