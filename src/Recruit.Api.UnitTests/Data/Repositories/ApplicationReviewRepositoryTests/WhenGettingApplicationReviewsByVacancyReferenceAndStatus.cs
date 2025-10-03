using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ApplicationReviewRepositoryTests;
[TestFixture]
internal class WhenGettingApplicationReviewsByVacancyReferenceAndStatus
{
    [Test, MoqAutoData]
    public async Task GettingApplicationReviewsByVacancyReferenceAndStatus_ShouldReturnApplicationReviews(
        long vacancyReference,
        ApplicationReviewStatus status,
        CancellationToken token,
        List<ApplicationReviewEntity> entities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] ApplicationReviewRepository repository)
    {
        // Arrange
        foreach (var applicationReviewEntity in entities)
        {
            applicationReviewEntity.VacancyReference = vacancyReference;
            applicationReviewEntity.Status = status;
        }
        context.Setup(x => x.ApplicationReviewEntities)
            .ReturnsDbSet(entities);
        // Act
        var result = await repository.GetAllByVacancyReferenceAndStatus(vacancyReference, status, false, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(entities);
    }

    [Test, RecursiveMoqAutoData]
    public async Task GettingApplicationReviewsByVacancyReferenceAndStatus_Should_Return_List(
        long vacancyReference,
        ApplicationReviewStatus status,
        CancellationToken token,
        List<ApplicationReviewEntity> entities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] ApplicationReviewRepository repository)
    {
        // Arrange
        foreach (var applicationReviewEntity in entities)
        {
            applicationReviewEntity.VacancyReference = vacancyReference;
            applicationReviewEntity.TemporaryReviewStatus = status;
        }
        context.Setup(x => x.ApplicationReviewEntities)
            .ReturnsDbSet(entities);
        // Act
        var result = await repository.GetAllByVacancyReferenceAndStatus(vacancyReference, status, true, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(entities);
    }
}
