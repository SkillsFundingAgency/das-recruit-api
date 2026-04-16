using System.Net;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Contracts.ApiRequests;
using SFA.DAS.Recruit.Contracts.ApiResponses;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyReferenceControllerTests;

public class WhenGettingVacancyReference: BaseFixture
{
    [Test]
    public async Task Then_Without_Required_Fields_Bad_Request_Is_Returned()
    {
        // arrange
        var vacancyReference = Fixture.Create<VacancyReference>();
        Server.DataContext.Setup(x => x.GetNextVacancyReferenceAsync(It.IsAny<CancellationToken>())).ReturnsAsync(vacancyReference.Value);
        
        // act
        var response = await Client.GetAsync(new GetVacancyreferenceApiRequest().GetUrl);
        var result = await response.Content.ReadAsAsync<VacancyReferenceResponse>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.NextVacancyReference.Should().Be(vacancyReference.Value);
    }
}