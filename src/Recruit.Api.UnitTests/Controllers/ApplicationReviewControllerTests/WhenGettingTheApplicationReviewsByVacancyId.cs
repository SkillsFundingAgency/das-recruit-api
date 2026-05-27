using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ApplicationReviewControllerTests;

[TestFixture]
internal class WhenGettingTheApplicationReviewsByVacancyId
{
    [Test, RecursiveMoqAutoData]
    public async Task Get_ReturnsOk_WhenApplicationReviewExists(
        Guid vacancyId,
        List<ApplicationReviewEntity> mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetAllByVacancyId(vacancyId, token)).ReturnsAsync(mockResponse);

        // Act
        var result = await controller.GetManyByVacancyId(vacancyId, token);

        // Assert
        result.Should().BeOfType<Ok<List<GetApplicationReviewResponse>>>();
        var okResult = result as Ok<List<GetApplicationReviewResponse>>;
        okResult!.Value.Should().BeEquivalentTo(mockResponse.ToGetResponse());
    }

    [Test, RecursiveMoqAutoData]
    public async Task Get_ReturnsNotFound_WhenApplicationReviewDoesNotExist(
        Guid vacancyId,
        List<ApplicationReviewEntity> mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetAllByVacancyId(vacancyId, token)).ReturnsAsync((List<ApplicationReviewEntity>)null!);

        // Act
        var result = await controller.GetManyByVacancyId(vacancyId, token);

        // Assert
        result.Should().BeOfType<NotFound>();
    }

    [Test, RecursiveMoqAutoData]
    public async Task Get_ReturnsInternalServerException_WhenException_Thrown(
        Guid vacancyId,
        List<ApplicationReviewEntity> mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetAllByVacancyId(vacancyId, token)).ThrowsAsync(new Exception());

        // Act
        var result = await controller.GetManyByVacancyId(vacancyId, token);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}
