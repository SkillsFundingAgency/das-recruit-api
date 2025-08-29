using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.VacancyReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.VacancyReviewRepositoryTests;

[TestFixture]
internal class WhenGettingQaDashboard
{
    [Test]
    [MoqInlineAutoData(ReviewStatus.UnderReview)]
    [MoqInlineAutoData(ReviewStatus.PendingReview)]
    public async Task Then_GetQaDashboard_Return_As_Expected(ReviewStatus status,
            CancellationToken token,
            List<VacancyReviewEntity> entities,
            [Frozen] Mock<IRecruitDataContext> context,
            [Greedy] VacancyReviewRepository repository)
    {
        // Arrange
        foreach (var vacancyReviewEntity in entities)
        {
            vacancyReviewEntity.Status = status;
        }

        context.Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(entities);

        // Act
        var result = await repository.GetQaDashboard(token);

        // Assert
        result.TotalVacanciesForReview.Should().Be(entities.Count);
    }

    [Test]
    [MoqInlineAutoData(ReviewStatus.New)]
    [MoqInlineAutoData(ReviewStatus.Closed)]
    public async Task Then_GetQaDashboard_Return_As_Empty(ReviewStatus status,
            CancellationToken token,
            List<VacancyReviewEntity> entities,
            [Frozen] Mock<IRecruitDataContext> context,
            [Greedy] VacancyReviewRepository repository)
    {
        // Arrange
        foreach (var vacancyReviewEntity in entities)
        {
            vacancyReviewEntity.Status = status;
        }

        context.Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(entities);

        // Act
        var result = await repository.GetQaDashboard(token);

        // Assert
        result.TotalVacanciesForReview.Should().Be(0);
    }
}
