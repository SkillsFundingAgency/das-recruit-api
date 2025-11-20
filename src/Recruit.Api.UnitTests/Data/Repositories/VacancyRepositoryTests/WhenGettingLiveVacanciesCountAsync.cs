using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.VacancyRepositoryTests;
[TestFixture]
internal class WhenGettingLiveVacanciesCountAsync
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Live_Vacancies_Count_Is_Returned_Given_ClosingDate(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyRepository sut,
        CancellationToken token)
    {
        //Arrange
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
        int actualCount = await sut.GetLiveVacanciesCountAsync(token);
        
        //Assert
        actualCount.Should().Be(1);
    }
}
