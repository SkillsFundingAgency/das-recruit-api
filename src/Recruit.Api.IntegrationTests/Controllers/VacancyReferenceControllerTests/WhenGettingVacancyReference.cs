using System.Net;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Responses;
using SFA.DAS.Recruit.Api.Testing.Http;

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
        var response = await Client.GetAsync(RouteNames.VacancyReference);
        var result = await response.Content.ReadAsAsync<VacancyReferenceResponse>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.NextVacancyReference.Should().Be(vacancyReference.Value);
    }
}