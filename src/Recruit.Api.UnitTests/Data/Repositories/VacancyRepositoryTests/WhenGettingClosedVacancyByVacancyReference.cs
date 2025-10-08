using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.VacancyRepositoryTests;
[TestFixture]
internal class WhenGettingClosedVacancyByVacancyReference
{
    [Test, RecursiveMoqAutoData]
    public async Task GetClosedVacancyByVacancyReference_ShouldReturnClosedVacancy_WhenItExists(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyRepository repository,
        CancellationToken token)
    {
        // Arrange
        VacancyReference vacancyReference = 10000000;
        var data = new List<VacancyEntity>
        {
            new() { VacancyReference = vacancyReference.Value, Status = VacancyStatus.Closed },
            new() { VacancyReference = 99999, Status = VacancyStatus.Live }
        }.AsQueryable();

        context.Setup(x => x.VacancyEntities)
            .ReturnsDbSet(data);

        // Act
        var result = await repository.GetOneClosedVacancyByVacancyReference(vacancyReference, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.VacancyReference.Should().Be(10000000L);
        result.Status.Should().Be(VacancyStatus.Closed);
    }

    [Test, RecursiveMoqAutoData]
    public async Task GetClosedVacancyByVacancyReference_ShouldReturnLiveVacancy_WhenItExists(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyRepository repository,
        CancellationToken token)
    {
        // Arrange
        VacancyReference vacancyReference = 10000000;

        var data = new List<VacancyEntity>
        {
            new() { VacancyReference = 12345, Status = VacancyStatus.Closed },
            new() { VacancyReference = vacancyReference.Value, Status = VacancyStatus.Live }
        }.AsQueryable();

        context.Setup(x => x.VacancyEntities)
            .ReturnsDbSet(data);

        // Act
        var result = await repository.GetOneClosedVacancyByVacancyReference(vacancyReference, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(VacancyStatus.Live);
    }

    [Test, RecursiveMoqAutoData]
    public async Task GetClosedVacancyByVacancyReference_ShouldReturnNull_WhenVacancyNotFound(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyRepository repository,
        CancellationToken token)
    {
        // Arrange
        VacancyReference vacancyReference = 10000000;

        var data = new List<VacancyEntity>
        {
            new() { VacancyReference = 12345, Status = VacancyStatus.Closed },
            new() { VacancyReference = 99999, Status = VacancyStatus.Live }
        }.AsQueryable();

        context.Setup(x => x.VacancyEntities)
            .ReturnsDbSet(data);

        // Act
        var result = await repository.GetOneClosedVacancyByVacancyReference(vacancyReference, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
