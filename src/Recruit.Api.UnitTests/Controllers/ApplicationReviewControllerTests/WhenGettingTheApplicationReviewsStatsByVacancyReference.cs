using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ApplicationReviewControllerTests;
[TestFixture]
internal class WhenGettingTheApplicationReviewsStatsByVacancyReference
{
    [Test, MoqAutoData]
    public async Task Get_ReturnsOk_WhenApplicationReviewsExist(
        long vacancyReference,
        ApplicationReviewsStats mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetStatusCountByVacancyReference(vacancyReference, token)).ReturnsAsync(mockResponse);
        // Act
        var result = await controller.GetStatusCountByVacancyReference(vacancyReference, token);
        // Assert
        result.Should().BeOfType<Ok<ApplicationReviewsStats>>();
        var okResult = result as Ok<ApplicationReviewsStats>;
        okResult!.Value.Should().BeEquivalentTo(mockResponse);
    }

   [Test, MoqAutoData]
    public async Task Get_ReturnsInternalServerException_WhenException_Thrown(
        long vacancyReference,
        List<ApplicationReviewEntity> mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider
            .Setup(p => p.GetStatusCountByVacancyReference(vacancyReference, token))
            .ThrowsAsync(new Exception());
        // Act
        var result =
            await controller.GetStatusCountByVacancyReference(vacancyReference, token);
        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}
