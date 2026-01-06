using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.VacancyReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Testing.Data;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.VacancyReviewRepositoryTests;

[TestFixture]
internal class WhenGettingQaDashboard
{
    [Test]
    [MoqInlineAutoData(ReviewStatus.UnderReview)]
    [MoqInlineAutoData(ReviewStatus.PendingReview)]
    public async Task Then_GetQaDashboard_Return_As_Expected(ReviewStatus status,            
            List<VacancyReviewEntity> entities,
            [Frozen] Mock<IRecruitDataContext> context,
            [Greedy] VacancyReviewRepository repository,
            CancellationToken token)
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
    [MoqInlineAutoData(ReviewStatus.UnderReview)]
    [MoqInlineAutoData(ReviewStatus.PendingReview)]
    public async Task GetQaDashboard_Should_Count_TotalVacanciesForReview(ReviewStatus status,
        List<VacancyReviewEntity> entities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyReviewRepository repository,
        CancellationToken token)
    {
        // Arrange: 2 vacancies under review
        foreach (var vacancyReviewEntity in entities)
        {
            vacancyReviewEntity.Status = status;
            vacancyReviewEntity.CreatedDate = DateTime.UtcNow.AddHours(-5);
            vacancyReviewEntity.SlaDeadLine = DateTime.UtcNow.AddHours(-1);
            vacancyReviewEntity.SubmissionCount = 1;
        }

        context.Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(entities);

        // Act
        var result = await repository.GetQaDashboard(CancellationToken.None);

        // Assert
        result.TotalVacanciesForReview.Should().Be(entities.Count);
    }

    [Test]
    [MoqInlineAutoData(ReviewStatus.UnderReview)]
    [MoqInlineAutoData(ReviewStatus.PendingReview)]
    public async Task GetQaDashboard_Should_Count_ResubmittedVacancies(ReviewStatus status,
        List<VacancyReviewEntity> entities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyReviewRepository repository,
        CancellationToken token)
    {

        // Arrange
        foreach (var vacancyReviewEntity in entities)
        {
            vacancyReviewEntity.Status = status;
            vacancyReviewEntity.CreatedDate = DateTime.UtcNow.AddHours(-10);
            vacancyReviewEntity.SlaDeadLine = DateTime.UtcNow.AddHours(-1);
            vacancyReviewEntity.SubmissionCount = 2;
        }

        context.Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(entities);
        
        // Act
        var result = await repository.GetQaDashboard(CancellationToken.None);

        // Assert
        result.TotalVacanciesResubmitted.Should().Be(entities.Count);
    }

    [Test]
    [MoqInlineAutoData(ReviewStatus.UnderReview)]
    [MoqInlineAutoData(ReviewStatus.PendingReview)]
    public async Task GetQaDashboard_Should_Count_BrokenSla(ReviewStatus status,
        List<VacancyReviewEntity> entities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyReviewRepository repository,
        CancellationToken token)
    {
        // Arrange
        foreach (var vacancyReviewEntity in entities)
        {
            vacancyReviewEntity.Status = status;
            vacancyReviewEntity.CreatedDate = DateTime.UtcNow.AddHours(-30); // older than 24h;
            vacancyReviewEntity.SlaDeadLine = DateTime.UtcNow.AddHours(-5);
            vacancyReviewEntity.SubmissionCount = 1;
        }


        context.Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(entities);

        // Act
        var result = await repository.GetQaDashboard(CancellationToken.None);

        // Assert
        result.TotalVacanciesBrokenSla.Should().Be(entities.Count);
    }

    [Test]
    [MoqInlineAutoData(ReviewStatus.UnderReview)]
    [MoqInlineAutoData(ReviewStatus.PendingReview)]
    public async Task GetQaDashboard_Should_Count_SubmissionsLast12Hours(ReviewStatus status,
        List<VacancyReviewEntity> entities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyReviewRepository repository,
        CancellationToken token)
    {
        // Arrange
        foreach (var vacancyReviewEntity in entities)
        {
            vacancyReviewEntity.Status = status;
            vacancyReviewEntity.CreatedDate = DateTime.UtcNow.AddHours(-6); // older than 24h;
            vacancyReviewEntity.SlaDeadLine = DateTime.UtcNow.AddHours(+6);
            vacancyReviewEntity.SubmissionCount = 1;
        }


        context.Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(entities);

        // Act
        var result = await repository.GetQaDashboard(CancellationToken.None);

        // Assert
        result.TotalVacanciesSubmittedLastTwelveHours.Should().Be(entities.Count);
    }

    [Test]
    [MoqInlineAutoData(ReviewStatus.UnderReview)]
    [MoqInlineAutoData(ReviewStatus.PendingReview)]
    public async Task GetQaDashboard_Should_Count_SubmissionsTwelveToTwentyFourHours(ReviewStatus status,
        List<VacancyReviewEntity> entities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyReviewRepository repository,
        CancellationToken token)
    {
        // Arrange
        foreach (var vacancyReviewEntity in entities)
        {
            vacancyReviewEntity.Status = status;
            vacancyReviewEntity.CreatedDate = DateTime.UtcNow.AddHours(-15); // older than 24h;
            vacancyReviewEntity.SlaDeadLine = DateTime.UtcNow.AddHours(+5);
            vacancyReviewEntity.SubmissionCount = 1;
        }


        context.Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(entities);

        // Act
        var result = await repository.GetQaDashboard(CancellationToken.None);

        // Assert
        result.TotalVacanciesSubmittedTwelveTwentyFourHours.Should().Be(entities.Count);
    }

    [Test]
    [MoqInlineAutoData(ReviewStatus.New)]
    [MoqInlineAutoData(ReviewStatus.Closed)]
    public async Task Then_GetQaDashboard_Return_As_Empty(ReviewStatus status,            
            List<VacancyReviewEntity> entities,
            [Frozen] Mock<IRecruitDataContext> context,
            [Greedy] VacancyReviewRepository repository,
            CancellationToken token)
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
