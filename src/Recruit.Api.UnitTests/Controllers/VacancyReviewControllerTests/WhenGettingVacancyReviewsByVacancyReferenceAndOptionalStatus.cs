using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.VacancyReview;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyReviewControllerTests;

[TestFixture]
internal class WhenGettingVacancyReviewsByVacancyReferenceAndOptionalStatus
{
    [Test, RecruitAutoData]
    public async Task Then_The_List_Of_VacancyReviews_Is_Returned_With_Status_Filter(
        VacancyReference vacancyReference,
        List<SFA.DAS.Recruit.Api.Domain.Entities.VacancyReviewEntity> entities,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        var statuses = new List<ReviewStatus> { ReviewStatus.PendingReview };

        repository
            .Setup(x => x.GetManyByVacancyReferenceAndStatus(
                It.IsAny<VacancyReference>(),
                It.IsAny<IReadOnlyCollection<ReviewStatus>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await sut.GetManyByVacancyReference(repository.Object, vacancyReference, statuses, token);

        repository.Verify(x => x.GetManyByVacancyReferenceAndStatus(vacancyReference, statuses, token), Times.Once);
        result.Should().BeOfType<Ok<List<VacancyReview>>>();
    }

    [Test, RecruitAutoData]
    public async Task Then_Empty_List_Returned_When_No_Matches(
        VacancyReference vacancyReference,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        var empty = new List<SFA.DAS.Recruit.Api.Domain.Entities.VacancyReviewEntity>();
        List<ReviewStatus>? statuses = null;

        repository
            .Setup(x => x.GetManyByVacancyReferenceAndStatus(
                It.IsAny<VacancyReference>(),
                It.IsAny<IReadOnlyCollection<ReviewStatus>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(empty);

        var result = await sut.GetManyByVacancyReference(repository.Object, vacancyReference, statuses, token);

        repository.Verify(x => x.GetManyByVacancyReferenceAndStatus(vacancyReference, It.Is<IReadOnlyCollection<ReviewStatus>>(s => s.Count == 0), token), Times.Once);
        var ok = result.Should().BeOfType<Ok<List<VacancyReview>>>().Subject;
        ok.Value.Should().NotBeNull();
        ok.Value!.Count.Should().Be(0);
    }
}
