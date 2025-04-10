using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace Recruit.Api.UnitTests.Controllers.ProviderAccountControllerTests
{
    [TestFixture]
    public class WhenGettingCountByAccountId
    {
        [Test, MoqAutoData]
        public async Task Then_The_Count_ReturnsOk(
            int ukprn,
            List<long> vacancyReferences,
            List<ApplicationReviewsStats> mockResponse,
            CancellationToken token,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Greedy] ProviderAccountController controller)
        {
            // Arrange
            providerMock.Setup(p => p.GetVacancyReferencesCountByUkprn(ukprn, vacancyReferences, token))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await controller.GetCountByVacancyReferences(ukprn, vacancyReferences, token);

            // Assert
            result.Should().BeOfType<Ok<List<ApplicationReviewsStats>>>();
            var okResult = result as Ok<List<ApplicationReviewsStats>>;

            okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value!.Should().BeEquivalentTo(mockResponse);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Exception(
            int ukprn,
            List<long> vacancyReferences,
            List<ApplicationReviewsStats> mockResponse,
            CancellationToken token,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Greedy] ProviderAccountController controller)
        {
            // Arrange
            // Arrange
            providerMock.Setup(p => p.GetVacancyReferencesCountByUkprn(ukprn, vacancyReferences, token))
                .ThrowsAsync(new Exception());

            // Act
            var result = await controller.GetCountByVacancyReferences(ukprn, vacancyReferences, token);

            // Assert
            result.Should().BeOfType<ProblemHttpResult>();
        }
    }
}
