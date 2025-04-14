using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.ProhibitedContentControllerTests;

public class WhenGettingProhibitedContent: BaseFixture
{
    [Test]
    public async Task Then_If_No_ContentType_Is_Specified_Bad_Request_Is_Returned()
    {
        // act
        var response = await Client.GetAsync("api/prohibitedcontent/foo");
        var payload = await response.Content.ReadAsAsync<ValidationProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        payload.Should().NotBeNull();
        payload.Errors.Should().HaveCount(1);
        payload.Errors.Should().ContainKey("contentType");
    }
}