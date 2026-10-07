using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Contracts.ApiRequests;
using SFA.DAS.Recruit.Contracts.ApiResponses;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyControllerTests;

public class WhenGettingVacancy: MsSqlBaseFixture
{
    [Test]
    public async Task Then_The_Vacancy_Is_Returned()
    {
        // arrange
        var items = await DbData.CreateMany<VacancyEntity>(10);
        var expected = items[new Random().Next(items.Count)];

        // act
        var response = await Measure.ThisAsync(async () => await Client.GetAsync(new GetVacanciesByVacancyIdApiRequest(expected.Id).GetUrl));
        response.EnsureSuccessStatusCode();
        var vacancy = await response.ReadContentAsAsync<Vacancy>();

        // assert
        response.Is.Ok();
        vacancy.Should().NotBeNull();
        vacancy.Should().BeEquivalentTo(expected.ToGetResponse());
    }
    
    [Test]
    public async Task Then_The_Vacancy_Is_NotFound()
    {
        // arrange
        await DbData.CreateMany<VacancyEntity>(10);

        // act
        var response = await Measure.ThisAsync(async () => await Client.GetAsync(new GetVacanciesByVacancyIdApiRequest(Guid.NewGuid()).GetUrl));

        // assert
        response.Is.NotFound();
    }
}