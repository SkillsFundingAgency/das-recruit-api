using Recruit.Api.UnitTests.Data.DatabaseMock;
using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace Recruit.Api.UnitTests.Data.ApplicationReviewRepositoryTests;

[TestFixture]
public class WhenGettingAllByUkprn
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_ApplicationReviews_Are_Returned_By_Ukprn(
        int ukprn,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        CancellationToken token,
        List<ApplicationReviewEntity> applicationsReviews,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] ApplicationReviewRepository repository)
    {
        sortColumn = "CreatedDate";
        pageNumber = 1;
        pageSize = 10;
        foreach (var application in applicationsReviews)
        {
            application.Ukprn = ukprn;
        }

        var allApplications = new List<ApplicationReviewEntity>();
        allApplications.AddRange(applicationsReviews);

        context.Setup(x => x.ApplicationReviewEntities)
            .ReturnsDbSet(allApplications);

        var actual = await repository.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token);

        actual.Items.Should().BeEquivalentTo(applicationsReviews);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_The_ApplicationReviews_Are_Returned_By_Ukprn(
        int ukprn,
        ApplicationStatus status,
        CancellationToken token,
        List<ApplicationReviewEntity> applicationsReviews,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] ApplicationReviewRepository repository)
    {
        foreach (var application in applicationsReviews)
        {
            application.Ukprn = ukprn;
            application.Status = status.ToString();
        }

        var allApplications = new List<ApplicationReviewEntity>();
        allApplications.AddRange(applicationsReviews);

        context.Setup(x => x.ApplicationReviewEntities)
            .ReturnsDbSet(allApplications);

        var actual = await repository.GetAllByUkprn(ukprn, status.ToString(), token);

        actual.Should().BeEquivalentTo(applicationsReviews);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_The_ApplicationReviews_Are_Returned_By_Ukprn(
        int ukprn,
        ApplicationStatus status,
        long vacancyReference,
        CancellationToken token,
        List<ApplicationReviewEntity> applicationsReviews,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] ApplicationReviewRepository repository)
    {
        foreach (var application in applicationsReviews)
        {
            application.Ukprn = ukprn;
            application.VacancyReference = vacancyReference;
            application.Status = status.ToString();
        }

        var allApplications = new List<ApplicationReviewEntity>();
        allApplications.AddRange(applicationsReviews);

        context.Setup(x => x.ApplicationReviewEntities)
            .ReturnsDbSet(allApplications);

        var actual = await repository.GetAllByUkprn(ukprn, [vacancyReference], token);

        actual.Should().BeEquivalentTo(applicationsReviews);
    }
}