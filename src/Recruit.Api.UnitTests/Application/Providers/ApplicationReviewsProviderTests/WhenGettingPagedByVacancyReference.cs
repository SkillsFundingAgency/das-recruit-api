using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Application.Providers.ApplicationReviewsProviderTests;
[TestFixture]
internal class WhenGettingPagedByVacancyReference
{
    [Test, MoqAutoData]
    public async Task GetPagedByVacancyReferenceAsync_CallsRepositoryWithCorrectParameters(
        long vacancyReference,
        List<ApplicationReviewEntity> applicationReviews,
        [Frozen] Mock<IApplicationReviewRepository> repository,
        [Greedy] ApplicationReviewsProvider provider)
    {
        // Arrange
        int pageNumber = 2;
        int pageSize = 5;
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate);
        bool isAscending = true;
        var token = CancellationToken.None;
        var pagedList = new PaginatedList<ApplicationReviewEntity>(applicationReviews, applicationReviews.Count, pageNumber, pageSize);

        repository.Setup(r => r.GetPagedByVacancyReference(vacancyReference, pageNumber, pageSize, sortColumn, isAscending, token))
            .ReturnsAsync(pagedList);

        // Act
        var result = await provider.GetPagedByVacancyReferenceAsync(vacancyReference, pageNumber, pageSize, sortColumn, isAscending, token);

        // Assert
        result.Should().BeEquivalentTo(pagedList);
        repository.Verify(r => r.GetPagedByVacancyReference(vacancyReference, pageNumber, pageSize, sortColumn, isAscending, token), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task GetPagedByVacancyReferenceAsync_UsesDefaultParameters(
        long vacancyReference,
        [Frozen] Mock<IApplicationReviewRepository> repository,
        [Greedy] ApplicationReviewsProvider provider)
    {
        // Arrange
        var pagedList = new PaginatedList<ApplicationReviewEntity>([], 0, 1, 10);

        repository.Setup(r => r.GetPagedByVacancyReference(vacancyReference, 1, 10, nameof(ApplicationReviewEntity.CreatedDate), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedList);

        // Act
        var result = await provider.GetPagedByVacancyReferenceAsync(vacancyReference);

        // Assert
        result.Should().BeEquivalentTo(pagedList);
        repository.Verify(r => r.GetPagedByVacancyReference(vacancyReference, 1, 10, nameof(ApplicationReviewEntity.CreatedDate), false, It.IsAny<CancellationToken>()), Times.Once);
    }
}
