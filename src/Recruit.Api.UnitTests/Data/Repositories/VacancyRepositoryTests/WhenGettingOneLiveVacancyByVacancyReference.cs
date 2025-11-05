using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.VacancyRepositoryTests;
[TestFixture]
internal class WhenGettingOneLiveVacancyByVacancyReference
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Live_Vacancy_Is_Returned_Given_A_Valid_VacancyReference(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] VacancyRepository sut,
        CancellationToken token)
    {
        //Arrange
        VacancyReference vacancyReference = 10000000;
        var data = new List<VacancyEntity>
        {
            new() { VacancyReference = vacancyReference.Value, Status = VacancyStatus.Live, ClosingDate = DateTime.UtcNow.AddMinutes(1) },
        }.AsQueryable();

        context.Setup(x => x.VacancyEntities)
            .ReturnsDbSet(data);

        //Act
        var actualVacancy = await sut.GetOneLiveVacancyByVacancyReferenceAsync(vacancyReference, token);
        
        //Assert
        actualVacancy.Should().NotBeNull();
        actualVacancy.VacancyReference.Should().Be(10000000L);
        actualVacancy.Status.Should().Be(VacancyStatus.Live);
    }
}