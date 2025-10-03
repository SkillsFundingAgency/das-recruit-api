using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ApplicationReviewControllerTests;
[TestFixture]
internal class WhenGettingTheApplicationReviewsByVacancyReferenceAndCandidateId
{
    [Test, MoqAutoData]
    public async Task Get_ReturnsOk_WhenApplicationReviewExists(
        long vacancyReference,
        Guid candidateId,
        ApplicationReviewEntity mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetByVacancyReferenceAndCandidateId(vacancyReference, candidateId, token)).ReturnsAsync(mockResponse);

        // Act
        var result = await controller.GetOneByVacancyReferenceAndCandidateId(vacancyReference, candidateId, token);

        // Assert
        result.Should().BeOfType<Ok<GetApplicationReviewResponse>>();
        var okResult = result as Ok<GetApplicationReviewResponse>;
        okResult!.Value.Should().BeEquivalentTo(mockResponse.ToGetResponse());
    }

    [Test, MoqAutoData]
    public async Task Get_ReturnsNotFound_WhenApplicationReviewDoesNotExist(
        long vacancyReference,
        Guid candidateId,
        ApplicationReviewEntity mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetByVacancyReferenceAndCandidateId(vacancyReference, candidateId, token)).ReturnsAsync((ApplicationReviewEntity)null!);

        // Act
        var result = await controller.GetOneByVacancyReferenceAndCandidateId(vacancyReference, candidateId, token);

        // Assert
        result.Should().BeOfType<NotFound>();
    }

    [Test, MoqAutoData]
    public async Task Get_ReturnsInternalServerException_WhenException_Thrown(
        long vacancyReference,
        Guid candidateId,
        ApplicationReviewEntity mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        provider.Setup(p => p.GetByVacancyReferenceAndCandidateId(vacancyReference, candidateId, token)).ThrowsAsync(new Exception());

        // Act
        var result = await controller.GetOneByVacancyReferenceAndCandidateId(vacancyReference, candidateId, token);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}
