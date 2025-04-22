using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Models.Requests.EmployerProfile;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.EmployerProfileControllerTests;

public class WhenPuttingAnEmployerProfile : BaseFixture
{
    [Test]
    public async Task Then_Without_Required_Fields_Bad_Request_Is_Returned()
    {
        // act
        var response = await Client.PutAsJsonAsync($"{RouteNames.EmployerProfile}/1", new {});
        var errors = await response.Content.ReadAsAsync<ValidationProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Should().NotBeNull();
        errors.Errors.Should().HaveCount(1);
        errors.Errors.Should().ContainKey(nameof(PutEmployerProfileRequest.AccountId));
    }
}