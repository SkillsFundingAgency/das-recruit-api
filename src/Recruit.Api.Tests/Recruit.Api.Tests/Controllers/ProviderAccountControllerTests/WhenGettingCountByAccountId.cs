using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Recruit.Api.Application.Providers;
using Recruit.Api.Domain.Enums;
using Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Tests.Controllers.ProviderAccountControllerTests
{
    [TestFixture]
    public class WhenGettingCountByAccountId
    {
        [Test, MoqAutoData]
        public async Task Then_The_Count_ReturnsOk(
            int ukprn,
            List<long> vacancyReferences,
            ApplicationStatus status,
            List<ApplicationReviewsStats> mockResponse,
            CancellationToken token,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Greedy] ProviderAccountController controller)
        {
            // Arrange
            providerMock.Setup(p => p.GetVacancyReferencesCountByUkprn(ukprn, vacancyReferences, status, token))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await controller.GetCountByVacancyReferences(ukprn, status, vacancyReferences, token);

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
            ApplicationStatus status,
            List<ApplicationReviewsStats> mockResponse,
            CancellationToken token,
            [Frozen] Mock<IApplicationReviewsProvider> providerMock,
            [Greedy] ProviderAccountController controller)
        {
            // Arrange
            // Arrange
            providerMock.Setup(p => p.GetVacancyReferencesCountByUkprn(ukprn, vacancyReferences, status, token))
                .ThrowsAsync(new Exception());

            // Act
            var result = await controller.GetCountByVacancyReferences(ukprn, status, vacancyReferences, token);

            // Assert
            result.Should().BeOfType<ProblemHttpResult>();
        }
    }
}
