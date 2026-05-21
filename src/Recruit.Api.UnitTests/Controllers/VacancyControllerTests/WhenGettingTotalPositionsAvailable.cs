using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Models.Responses.Vacancy;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.VacancyControllerTests;
[TestFixture]
internal class WhenGettingTotalPositionsAvailable
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_Gets_Total_Positions_Available_From_Service(
        int totalPositions,
        Mock<IVacancyRepository> repository,
        [Greedy] VacancyController sut,
        CancellationToken token)
    {
        //Arrange
        repository.Setup(x => x.GetLiveVacanciesCountAsync(token))
            .ReturnsAsync(totalPositions);
        //Act
        var result = await sut.GetTotalPositionsAvailable(repository.Object, token);

        //Assert
        var payload = (result as Ok<TotalPositionsAvailableResponse>)?.Value;
        payload.Should().NotBeNull();
        payload.TotalPositionsAvailable.Should().Be(totalPositions);
    }
}
