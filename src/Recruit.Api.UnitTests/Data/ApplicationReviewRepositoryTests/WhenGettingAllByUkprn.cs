using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.ApplicationReviewRepositoryTests;

[TestFixture]
internal class WhenGettingAllByUkprn
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_ApplicationReviews_Are_Returned_By_Ukprn(
        long vacancyReference,
        int ukprn,
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
            application.Ukprn = ukprn;
            application.VacancyReference = vacancyReference;
        }
        foreach (var vacancy in vacancyReviewEntities)
        {
            vacancy.VacancyReference = vacancyReference;
        }

        context.Setup(x => x.ApplicationReviewEntities)
            .ReturnsDbSet(applicationsReviews);
        context.Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(vacancyReviewEntities);

        var actual = await repository.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token);

        actual.Items.Should().BeEquivalentTo(applicationsReviews);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_The_ApplicationReviews_Not_Matched_Then_No_Results_Returned(
        int ukprn,
        ApplicationReviewStatus status,
        long vacancyReference,
        CancellationToken token,
        List<ApplicationReviewEntity> applicationsReviews,
        List<VacancyEntity> vacancyEntities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] ApplicationReviewRepository repository)
    {
        foreach (var application in applicationsReviews)
        {
            application.Ukprn = ukprn;
            application.VacancyReference = vacancyReference;
            application.Status = status.ToString();
        }

        context.Setup(x => x.ApplicationReviewEntities)
            .ReturnsDbSet(applicationsReviews);
        context.Setup(x => x.VacancyEntities)
            .ReturnsDbSet(vacancyEntities);

        var actual = await repository.GetByUkprnAndVacancyReferencesAsync(ukprn, [vacancyReference], token);

        actual.Count.Should().Be(0);
    }
}