using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Testing.Data;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.VacancyRepositoryTests;
[TestFixture]
internal class WhenGettingManyLiveVacancies
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Live_Vacancies_Are_Returned_Given_A_Valid_PageParams(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyRepository sut,
        CancellationToken token)
    {
        //Arrange
        int page = 1; 
        int pageSize = 10;
        VacancyReference vacancyReference = 10000000;
        var data = new List<VacancyEntity>
        {
            new() { VacancyReference = vacancyReference.Value, Status = VacancyStatus.Live, ClosingDate = DateTime.UtcNow.AddMinutes(1) },
            new() { VacancyReference = vacancyReference.Value, Status = VacancyStatus.Referred, ClosingDate = DateTime.UtcNow.AddMinutes(1) },
            new() { VacancyReference = vacancyReference.Value, Status = VacancyStatus.Submitted, ClosingDate = DateTime.UtcNow.AddMinutes(1) },
        }.AsQueryable();

        context.Setup(x => x.VacancyEntities)
            .ReturnsDbSet(data);

        //Act
        var actualVacancy = await sut.GetManyLiveVacancies(page, pageSize, null, token);

        //Assert
        actualVacancy.Should().NotBeNull();
        actualVacancy.TotalCount.Should().Be(1);
        actualVacancy.Items.Count.Should().Be(1);
        actualVacancy.PageSize.Should().Be(pageSize);
        actualVacancy.PageIndex.Should().Be(page);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Live_Vacancies_Are_Returned_Given_ClosingDate_And_Valid_PageParams(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyRepository sut,
        CancellationToken token)
    {
        //Arrange
        const int page = 1;
        const int pageSize = 10;
        DateTime closingDate = DateTime.UtcNow.AddMinutes(2);
        VacancyReference vacancyReference = 10000000;
        var data = new List<VacancyEntity>
        {
            new() { VacancyReference = vacancyReference.Value, Status = VacancyStatus.Live, ClosingDate = DateTime.UtcNow.AddMinutes(1) },
            new() { VacancyReference = vacancyReference.Value, Status = VacancyStatus.Referred, ClosingDate = DateTime.UtcNow.AddMinutes(1) },
            new() { VacancyReference = vacancyReference.Value, Status = VacancyStatus.Submitted, ClosingDate = DateTime.UtcNow.AddMinutes(1) },
        }.AsQueryable();

        context.Setup(x => x.VacancyEntities)
            .ReturnsDbSet(data);

        //Act
        var actualVacancy = await sut.GetManyLiveVacancies(page, pageSize, closingDate, token);

        //Assert
        actualVacancy.Should().NotBeNull();
        actualVacancy.TotalCount.Should().Be(1);
        actualVacancy.Items.Count.Should().Be(1);
        actualVacancy.PageSize.Should().Be(pageSize);
        actualVacancy.PageIndex.Should().Be(page);
    }
}
