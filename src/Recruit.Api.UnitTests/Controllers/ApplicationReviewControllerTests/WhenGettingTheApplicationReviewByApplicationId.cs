using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ApplicationReviewControllerTests;

internal class WhenGettingTheApplicationReviewByApplicationId
{
    [Test, RecursiveMoqAutoData]
    public async Task Get_ReturnsOk_WhenApplicationReviewExists(
        Guid applicationId,
        ApplicationReviewEntity mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetByApplicationId(applicationId, token)).ReturnsAsync(mockResponse);

        // Act
        var result = await controller.GetByApplicationId(applicationId, token);

        // Assert
        result.Should().BeOfType<Ok<GetApplicationReviewResponse>>();
        var okResult = result as Ok<GetApplicationReviewResponse>;
        okResult!.Value.Should().BeEquivalentTo(mockResponse.ToPutResponse());
    }

    [Test, RecursiveMoqAutoData]
    public async Task Get_ReturnsNotFound_WhenApplicationReviewDoesNotExist(
        Guid applicationId,
        ApplicationReviewEntity mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetByApplicationId(applicationId, token)).ReturnsAsync((ApplicationReviewEntity)null!);

        // Act
        var result = await controller.GetByApplicationId(applicationId, token);

        // Assert
        result.Should().BeOfType<NotFound>();
    }

    [Test, RecursiveMoqAutoData]
    public async Task Get_ReturnsInternalServerException_WhenException_Thrown(
        Guid applicationId,
        ApplicationReviewEntity mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetByApplicationId(applicationId, token)).ThrowsAsync(new Exception());

        // Act
        var result = await controller.GetByApplicationId(applicationId, token);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}