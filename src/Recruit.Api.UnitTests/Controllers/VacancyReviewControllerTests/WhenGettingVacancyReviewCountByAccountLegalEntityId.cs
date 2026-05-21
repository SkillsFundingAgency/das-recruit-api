using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyReviewControllerTests;

[TestFixture]
internal class WhenGettingVacancyReviewCountByAccountLegalEntityId
{
    [Test, RecruitAutoData]
    public async Task Then_The_Count_Is_Returned(
        int resultCount,
        long accountLegalEntityId,
        List<ReviewStatus> statuses,
        List<string> manualOutcomes,
        EmployerNameOption? employerNameOption,
        [Frozen] Mock<IVacancyReviewRepository> repository,
        [Greedy] VacancyReviewController sut,
        CancellationToken token)
    {
        repository
            .Setup(x => x.GetCountByAccountLegalEntityId(
                accountLegalEntityId, 
                statuses, 
                manualOutcomes, 
                employerNameOption,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultCount);

        var response = await sut.GetCountByAccountLegalEntityId(
            repository.Object,
            accountLegalEntityId,
            statuses,
            manualOutcomes,
            employerNameOption,
            token) as Ok<int>;

        (response?.Value).Should().Be(resultCount);
    }
}
