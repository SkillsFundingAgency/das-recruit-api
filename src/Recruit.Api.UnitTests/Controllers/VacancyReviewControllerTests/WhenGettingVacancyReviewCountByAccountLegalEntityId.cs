using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.VacancyReview;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyReviewControllerTests;

[TestFixture]
internal class WhenGettingVacancyReviewCountByAccountLegalEntityId
{
    [Test, RecruitAutoData]
    public async Task Then_The_Count_Is_Returned(
        int resultCount,
        Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        var accountLegalEntityId = 123456L;

        var statuses = new List<ReviewStatus> { ReviewStatus.Closed };
        var manualOutcomes = new List<string> { "Approved" };
        EmployerNameOption? employerNameOption = EmployerNameOption.Anonymous;

        repository
            .Setup(x => x.GetCountByAccountLegalEntityId(
                accountLegalEntityId, 
                statuses, 
                manualOutcomes, 
                employerNameOption,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultCount);

        var result = await sut.GetCountByAccountLegalEntityId(
            repository.Object,
            accountLegalEntityId,
            statuses,
            manualOutcomes,
            employerNameOption,
            token);

        result.Should().Be(resultCount);
    }
}
