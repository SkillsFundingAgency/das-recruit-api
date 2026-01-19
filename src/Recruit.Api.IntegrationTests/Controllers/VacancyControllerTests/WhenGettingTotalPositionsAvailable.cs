using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models.Responses.Vacancy;
using SFA.DAS.Recruit.Api.Testing.Data;
using SFA.DAS.Recruit.Api.Testing.Http;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyControllerTests;

[TestFixture]
internal class WhenGettingTotalPositionsAvailable : BaseFixture
{

    [Test, MoqAutoData]
    public async Task Then_The_Vacancies_Count_Is_Returned(
        int totalPositionsAvailable)
    {
        // arrange
        var items = Fixture.CreateMany<VacancyEntity>(10).ToList();
        foreach (VacancyEntity entity in items)
        {
            entity.ClosingDate = DateTime.UtcNow.AddMinutes(1);
            entity.Status = VacancyStatus.Live;
            entity.NumberOfPositions = totalPositionsAvailable;
        }
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet(items);

        // act
        var response = await Client.GetAsync($"{RouteNames.Vacancies}/{RouteElements.TotalPositionsAvailable}");
        var payload = await response.Content.ReadAsAsync<TotalPositionsAvailableResponse>();

        // assert
        response.EnsureSuccessStatusCode();
        payload.Should().NotBeNull();
        payload.TotalPositionsAvailable.Should().Be(items.Count * totalPositionsAvailable);
    }
}