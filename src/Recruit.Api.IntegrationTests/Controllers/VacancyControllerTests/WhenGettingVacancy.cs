using System.Net;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyControllerTests;

public class WhenGettingVacancy: MsSqlBaseFixture
{
    [Test]
    public async Task Then_The_Vacancy_Is_Returned()
    {
        // arrange
        var items = await TestData.CreateMany<VacancyEntity>(10);
        var expected = items[new Random().Next(items.Count)];

        // act
        var response = await Measure.ThisAsync(async () => await Client.GetAsync($"{RouteNames.Vacancies}/{expected.Id}"));
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        response.EnsureSuccessStatusCode();
        vacancy.Should().NotBeNull();
        vacancy.Should().BeEquivalentTo(expected.ToGetResponse(), Tolerate.SqlDateTime<Vacancy>());
    }
    
    [Test]
    public async Task Then_The_Vacancy_Is_NotFound()
    {
        // arrange
        await TestData.CreateMany<VacancyEntity>(10);

        // act
        var response = await Measure.ThisAsync(async () => await Client.GetAsync($"{RouteNames.Vacancies}/{Guid.NewGuid()}"));

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}