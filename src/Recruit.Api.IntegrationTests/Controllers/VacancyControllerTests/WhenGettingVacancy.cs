using System.Net;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Contracts.ApiRequests;

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
        var request = new GetVacanciesByVacancyIdApiRequest(expected.Id);
        var response = await Client.GetAsync(request.GetUrl);
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        response.EnsureSuccessStatusCode();
        vacancy.Should().NotBeNull();
        vacancy.Should().BeEquivalentTo(expected.ToGetResponse());
    }
    
    [Test]
    public async Task Then_The_Vacancy_Is_NotFound()
    {
        // arrange
        await DbData.CreateMany<VacancyEntity>(10);

        // act
        var request = new GetVacanciesByVacancyIdApiRequest(Guid.NewGuid());
        var response = await Client.GetAsync(request.GetUrl);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
