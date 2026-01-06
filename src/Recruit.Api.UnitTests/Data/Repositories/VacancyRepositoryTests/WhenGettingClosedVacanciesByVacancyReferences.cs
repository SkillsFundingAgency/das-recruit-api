using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Testing.Data;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.VacancyRepositoryTests;
[TestFixture]
internal class WhenGettingClosedVacanciesByVacancyReferences
{
    [Test, RecursiveMoqAutoData]
    public async Task GetClosedVacanciesByVacancyReferences_ShouldReturnClosedVacancies_WhenTheyExist(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyRepository repository,
        CancellationToken token)
    {
        // Arrange
        var vacancyReferences = new List<long> { 10000000, 20000000, 30000000 };
        var data = new List<VacancyEntity>
        {
            new() { VacancyReference = 10000000, Status = VacancyStatus.Closed },
            new() { VacancyReference = 20000000, Status = VacancyStatus.Closed },
            new() { VacancyReference = 30000000, Status = VacancyStatus.Closed },
            new() { VacancyReference = 40000000, Status = VacancyStatus.Live }
        }.AsQueryable();
        context.Setup(x => x.VacancyEntities)
            .ReturnsDbSet(data);
        
        // Act
        var result = await repository.GetManyClosedVacanciesByVacancyReferences(vacancyReferences, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(3);
        result.All(v => vacancyReferences.Contains(v.VacancyReference.GetValueOrDefault())).Should().BeTrue();
        result.All(v => v.Status == VacancyStatus.Closed).Should().BeTrue();
    }
}
