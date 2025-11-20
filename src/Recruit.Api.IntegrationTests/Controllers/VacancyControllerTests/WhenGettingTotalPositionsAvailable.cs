using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyControllerTests;
[TestFixture]
internal class WhenGettingTotalPositionsAvailable : BaseFixture
{

    [Test]
    public async Task Then_The_Vacancies_Count_Is_Returned()
    {
        // arrange
        var items = Fixture.CreateMany<VacancyEntity>(10).ToList();
        foreach (VacancyEntity entity in items)
        {
            entity.ClosingDate = DateTime.UtcNow.AddMinutes(1);
            entity.Status = VacancyStatus.Live;
        }
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet(items);

        // act
        var response = await Client.GetAsync($"{RouteNames.Vacancies}/{RouteElements.TotalPositionsAvailable}");
        int totalPositionsAvailable = await response.Content.ReadAsAsync<int>();

        // assert
        response.EnsureSuccessStatusCode();
        totalPositionsAvailable.Should().Be(items.Count);
    }
}