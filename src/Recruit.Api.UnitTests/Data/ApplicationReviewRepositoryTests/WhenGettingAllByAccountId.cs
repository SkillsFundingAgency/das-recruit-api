using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.ApplicationReviewRepositoryTests;

[TestFixture]
internal class WhenGettingAllByAccountId
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_ApplicationReviews_Are_Returned_By_AccountId(
        long vacancyReference,
        long accountId,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        CancellationToken token,
        List<ApplicationReviewEntity> applicationsReviews,
        List<VacancyReviewEntity> vacancyReviewEntities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] ApplicationReviewRepository repository)
    {
        sortColumn = "CreatedDate";
        pageNumber = 1;
        pageSize = 10;
        foreach (var application in applicationsReviews)
        {
            application.AccountId = accountId;
            application.VacancyReference = vacancyReference;
        }
        foreach (var vacancyReview in vacancyReviewEntities)
        {
            vacancyReview.VacancyReference = vacancyReference;
            vacancyReview.Status = ReviewStatus.Closed;
            vacancyReview.ManualOutcome = "Approved";
        }

        context.Setup(x => x.ApplicationReviewEntities)
            .ReturnsDbSet(applicationsReviews);
        context.Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(vacancyReviewEntities);

        var actual = await repository.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

        actual.Items.Should().BeEquivalentTo(applicationsReviews);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_The_ApplicationReviews_Not_Matched_Then_No_Results_Returned(
        long accountId,
        long vacancyReference,
        CancellationToken token,
        List<ApplicationReviewEntity> applicationsReviews,
        List<VacancyReviewEntity> vacancyReviewEntities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] ApplicationReviewRepository repository)
    {
        foreach (var application in applicationsReviews)
        {
            application.AccountId = accountId;
            application.VacancyReference = vacancyReference;
        }

        context.Setup(x => x.ApplicationReviewEntities)
            .ReturnsDbSet(applicationsReviews);
        context.Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(vacancyReviewEntities);

        var actual = await repository.GetAllByAccountId(accountId, [vacancyReference], token);

        actual.Count.Should().Be(0);
    }
}