using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ApplicationReviewControllerTests;
[TestFixture]
internal class WhenGettingTheApplicationReviewsByVacancyReferenceAndStatus
{
    [Test, MoqAutoData]
    public async Task Get_ReturnsOk_WhenApplicationReviewsExist(
        long vacancyReference,
        ApplicationReviewStatus status,
        bool includeTemporaryStatus,
        List<ApplicationReviewEntity> mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetAllByVacancyReferenceAndStatus(vacancyReference, status, includeTemporaryStatus, token)).ReturnsAsync(mockResponse);
        // Act
        var result = await controller.GetManyByVacancyReferenceAndStatus(vacancyReference, status, includeTemporaryStatus, token);
        // Assert
        result.Should().BeOfType<Ok<List<GetApplicationReviewResponse>>>();
        var okResult = result as Ok<List<GetApplicationReviewResponse>>;
        okResult!.Value.Should().BeEquivalentTo(mockResponse.ToGetResponse());
    }
    
    [Test, MoqAutoData]
    public async Task Get_Returns_Empty_WhenNoApplicationReviewsExist(
        long vacancyReference,
        ApplicationReviewStatus status,
        bool includeTemporaryStatus,
        List<ApplicationReviewEntity> mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetAllByVacancyReferenceAndStatus(vacancyReference, status, includeTemporaryStatus, token)).ReturnsAsync([]);
        // Act
        var result = await controller.GetManyByVacancyReferenceAndStatus(vacancyReference, status, includeTemporaryStatus, token);
        // Assert
        result.Should().BeOfType<Ok<List<GetApplicationReviewResponse>>>();
        var okResult = result as Ok<List<GetApplicationReviewResponse>>;
        okResult!.Value.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public async Task Get_ReturnsInternalServerException_WhenException_Thrown(
        long vacancyReference,
        ApplicationReviewStatus status,
        bool includeTemporaryStatus,
        List<ApplicationReviewEntity> mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider
            .Setup(p => p.GetAllByVacancyReferenceAndStatus(vacancyReference, status, includeTemporaryStatus, token))
            .ThrowsAsync(new Exception());
        // Act
        var result = 
            await controller.GetManyByVacancyReferenceAndStatus(vacancyReference, status, includeTemporaryStatus,
                token);
        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}
