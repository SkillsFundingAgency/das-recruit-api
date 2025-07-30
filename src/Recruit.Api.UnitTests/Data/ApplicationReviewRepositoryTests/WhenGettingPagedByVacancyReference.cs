using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.ApplicationReviewRepositoryTests;
[TestFixture]
internal class WhenGettingPagedByVacancyReference
{
    [Test, RecursiveMoqAutoData]
    public async Task GettingPagedByVacancyReference(
        long vacancyReference,
        CancellationToken token,
        List<ApplicationReviewEntity> entities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] ApplicationReviewRepository repository)
    {
        // Arrange
        foreach (var applicationReviewEntity in entities)
        {
            applicationReviewEntity.VacancyReference = vacancyReference;
        }

        context.Setup(x => x.ApplicationReviewEntities)
            .ReturnsDbSet(entities);

        // Act
        var result = await repository.GetPagedByVacancyReference(vacancyReference, 1, 10, "CreatedDate", true, token);

        // Assert
        result.Items.Should().BeEquivalentTo(entities);
        result.TotalCount.Should().Be(entities.Count);
    }
}
