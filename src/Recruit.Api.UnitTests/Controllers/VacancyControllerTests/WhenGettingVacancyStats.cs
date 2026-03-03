using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyControllerTests;

public class WhenGettingVacancyStats
{
    [Test, MoqAutoData]
    public async Task Then_The_Employer_Stats_Are_Returned(
        long accountId,
        List<long> vacancyReferences,
        List<ApplicationReviewsStats> stats,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] VacancyController sut)
    {
        // arrange
        provider
            .Setup(x => x.GetVacancyReferencesCountByAccountId(accountId, vacancyReferences, null, CancellationToken.None))
            .ReturnsAsync(stats);
        
        var expectedData = stats.ToDictionary(x => x.VacancyReference);

        // act
        var result = await sut.GetEmployerVacancyApplicationStats(
            provider.Object,
            accountId,
            vacancyReferences,
            CancellationToken.None) as Ok<DataResponse<Dictionary<long, ApplicationReviewsStats>>>;

        // assert
        result.Should().NotBeNull();
        result.Value!.Data.Should().BeEquivalentTo(expectedData);
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Provider_Stats_Are_Returned(
        int ukprn,
        List<long> vacancyReferences,
        List<ApplicationReviewsStats> stats,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] VacancyController sut)
    {
        // arrange
        provider
            .Setup(x => x.GetVacancyReferencesCountByUkprn(ukprn, vacancyReferences, CancellationToken.None))
            .ReturnsAsync(stats);
        
        var expectedData = stats.ToDictionary(x => x.VacancyReference);

        // act
        var result = await sut.GetProviderVacancyApplicationStats(
            provider.Object,
            ukprn,
            vacancyReferences,
            CancellationToken.None) as Ok<DataResponse<Dictionary<long, ApplicationReviewsStats>>>;

        // assert
        result.Should().NotBeNull();
        result.Value!.Data.Should().BeEquivalentTo(expectedData);
    }
}