using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Providers.ApplicationReviewsProviderTests;

internal class WhenGettingVacancyApplicationsCountRequiringFeedback
{
    [Test, MoqAutoData]
    public async Task Then_The_Repository_Is_Called_And_The_Results_Returned(
        List<long> vacancyIds,
        Dictionary<long, int> results,
        [Frozen] Mock<IApplicationReviewRepository> repository,
        [Greedy] ApplicationReviewsProvider sut, 
        CancellationToken cancellationToken)
    {
        // arrange
        repository
            .Setup(x => x.GetVacancyApplicationsCountRequiringFeedback(vacancyIds, cancellationToken))
            .ReturnsAsync(results);

        // act
        var actual = await sut.GetVacancyApplicationsCountRequiringFeedback(vacancyIds, cancellationToken);

        // assert
        actual.Should().BeEquivalentTo(results);
    }
}