using System.Net;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Contracts.ApiRequests;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyControllerTests;

public class WhenGettingVacancy: BaseFixture
{
    [Test]
    public async Task Then_The_Vacancy_Is_Returned()
    {
        // arrange
        var items = Fixture.CreateMany<VacancyEntity>(10).ToList();
        var expected = items[1];
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet(items);

        // act
        var request = new GetVacanciesByVacancyIdApiRequest(expected.Id);
        var response = await Client.GetAsync(request.GetUrl);
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        response.EnsureSuccessStatusCode();
        vacancy.Should().NotBeNull();
    }
    
    [Test]
    public async Task Then_The_Vacancy_Is_NotFound()
    {
        // arrange
        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(Fixture.CreateMany<VacancyEntity>(10).ToList());

        // act
        var request = new GetVacanciesByVacancyIdApiRequest(Guid.NewGuid());
        var response = await Client.GetAsync(request.GetUrl);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}