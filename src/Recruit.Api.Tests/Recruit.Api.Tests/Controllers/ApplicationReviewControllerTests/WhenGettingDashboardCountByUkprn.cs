using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Recruit.Api.Application.Providers;
using Recruit.Api.Domain.Enums;
using Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Tests.Controllers.ApplicationReviewControllerTests
{
    [TestFixture]
    public class WhenGettingDashboardCountByUkprn
    {
        [Test, MoqAutoData]
        public async Task Then_The_Count_ReturnsOk(
            int ukprn,
            ApplicationStatus status,
            DashboardModel mockResponse,
            CancellationToken token,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Greedy] ApplicationReviewController controller)
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
            ApplicationStatus status,
            DashboardModel mockResponse,
            CancellationToken token,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Greedy] ApplicationReviewController controller)
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