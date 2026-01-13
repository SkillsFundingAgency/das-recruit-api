using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.VacancyReview;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyReviewControllerTests;

[TestFixture]
internal class WhenGettingVacancyReviewsByUserAndAssignationExpiry
{
    [Test, RecruitAutoData]
    public async Task Then_The_List_Of_VacancyReviews_Is_Returned(
        List<SFA.DAS.Recruit.Api.Domain.Entities.VacancyReviewEntity> entities,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        var userId = "someone@example.com";
        DateTime? assignationExpiry = DateTime.UtcNow;

        repository
            .Setup(x => x.GetManyByReviewedByUserEmailAndAssignationExpiry(
                It.IsAny<string>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await sut.GetManyByUser(repository.Object, userId, assignationExpiry, token);

        repository.Verify(x => x.GetManyByReviewedByUserEmailAndAssignationExpiry(userId, assignationExpiry, token), Times.Once);
        result.Should().BeOfType<Ok<List<VacancyReview>>>();
    }
}
