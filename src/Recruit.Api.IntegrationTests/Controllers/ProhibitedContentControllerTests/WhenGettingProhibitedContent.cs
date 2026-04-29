using System.Net;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Contracts.ApiRequests;
using SFA.DAS.Recruit.Contracts.ApiResponses;
using ValidationProblemDetails = Microsoft.AspNetCore.Mvc.ValidationProblemDetails;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.ProhibitedContentControllerTests;

public class WhenGettingProhibitedContent: BaseFixture
{
    [Test]
    public async Task Then_The_Prohibited_Content_Is_Returned()
    {
        // arrange
        List<ProhibitedContentEntity> items = [
            new() {Content = "test", ContentType = Domain.Models.ProhibitedContentType.BannedPhrases}
        ];
        Server.DataContext.Setup(x => x.ProhibitedContentEntities).ReturnsDbSet(items);
        
        // act
        var request = new GetProhibitedcontentByContentTypeApiRequest(ProhibitedContentType.BannedPhrases);
        var response = await Client.GetAsync(request.GetUrl);
        var payload = await response.Content.ReadAsAsync<List<string>>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        payload.Should().NotBeNull();
    }
    
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