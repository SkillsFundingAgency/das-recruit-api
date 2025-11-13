using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Testing.Data;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ApplicationReviewRepositoryTests;
[TestFixture]
internal class WhenGettingApplicationReviewsByVacancyReferenceAndCandidateId
{
    [Test, RecursiveMoqAutoData]
    public async Task GettingApplicationReviewsByVacancyReferenceAndCandidateId_ShouldReturnApplicationReview(
        long vacancyReference,
        Guid candidateId,
        CancellationToken token,
        ApplicationReviewEntity entity,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] ApplicationReviewRepository repository)
    {
        // Arrange
        entity.VacancyReference = vacancyReference;
        entity.CandidateId = candidateId;
        entity.WithdrawnDate = null;
        context.Setup(x => x.ApplicationReviewEntities)
            .ReturnsDbSet(new List<ApplicationReviewEntity> { entity });
        // Act
        var result = await repository.GetByVacancyReferenceAndCandidateId(vacancyReference, candidateId, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(entity);
    }
}
