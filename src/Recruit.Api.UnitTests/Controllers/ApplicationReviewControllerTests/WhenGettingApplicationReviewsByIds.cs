using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ApplicationReviewControllerTests;
[TestFixture]
internal class WhenGettingApplicationReviewsByIds
{

    [Test, MoqAutoData]
    public async Task Get_ReturnsOk_WhenApplicationReviewExists(
        List<Guid> applicationIds,
        List<ApplicationReviewEntity> mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetAllByIdAsync(applicationIds, token)).ReturnsAsync(mockResponse);

        // Act
        var result = await controller.GetManyByIds(applicationIds, token);

        // Assert
        result.Should().BeOfType<Ok<List<GetApplicationReviewResponse>>>();
        var okResult = result as Ok<List<GetApplicationReviewResponse>>;
        okResult!.Value.Should().BeEquivalentTo(mockResponse.ToGetResponse());
    }

    [Test, MoqAutoData]
    public async Task Get_Returns_Empty_WhenApplicationReviewDoesNotExist(
        List<Guid> applicationIds,
        List<ApplicationReviewEntity> mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetAllByIdAsync(applicationIds, token)).ReturnsAsync([]);

        // Act
        var result = await controller.GetManyByIds(applicationIds, token);

        // Assert
        result.Should().BeOfType<Ok<List<GetApplicationReviewResponse>>>();
        var okResult = result as Ok<List<GetApplicationReviewResponse>>;
        okResult!.Value.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public async Task Get_ReturnsInternalServerException_WhenException_Thrown(
        List<Guid> applicationIds,
        List<ApplicationReviewEntity> mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetAllByIdAsync(applicationIds, token)).ThrowsAsync(new Exception());

        // Act
        var result = await controller.GetManyByIds(applicationIds, token);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}
