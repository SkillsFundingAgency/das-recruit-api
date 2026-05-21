using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyReviewControllerTests;

[TestFixture]
internal class WhenGettingVacancyReviewsByAccountLegalEntityId
{
    [Test, RecruitAutoData]
    public async Task Then_The_List_Of_VacancyReviews_Is_Returned(
        List<SFA.DAS.Recruit.Api.Domain.Entities.VacancyReviewEntity> entities,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        var accountLegalEntityId = 123456L;

        repository
            .Setup(x => x.GetManyByAccountLegalEntityId(
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await sut.GetManyByAccountLegalEntityId(repository.Object, accountLegalEntityId, token);

        repository.Verify(x => x.GetManyByAccountLegalEntityId(accountLegalEntityId, token), Times.Once);
        result.Should().BeOfType<Ok<List<VacancyReview>>>();
    }
}
