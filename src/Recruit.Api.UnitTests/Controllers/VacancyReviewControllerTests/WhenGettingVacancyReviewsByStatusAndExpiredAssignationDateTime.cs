using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.VacancyReview;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyReviewControllerTests;

[TestFixture]
internal class WhenGettingVacancyReviewsByStatusAndExpiredAssignationDateTime
{
    [Test, RecruitAutoData]
    public async Task Then_The_List_Of_VacancyReviews_Is_Returned(
        List<SFA.DAS.Recruit.Api.Domain.Entities.VacancyReviewEntity> entities,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        var statuses = new List<ReviewStatus> { ReviewStatus.PendingReview, ReviewStatus.UnderReview };
        var cutoff = DateTime.UtcNow;

        repository
            .Setup(x => x.GetManyByStatusAndExpiredAssignationDateTime(
                It.IsAny<IReadOnlyCollection<ReviewStatus>>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await sut.GetMany(repository.Object, statuses, cutoff, token);

        repository.Verify(x => x.GetManyByStatusAndExpiredAssignationDateTime(statuses, cutoff, token), Times.Once);
        result.Should().BeOfType<Ok<List<VacancyReview>>>();
    }
}
