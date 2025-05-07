using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Models.Requests.EmployerProfileAddress;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.EmployerProfileAddressControllerTests;

public class WhenPostingAnEmployerProfileAddress : BaseFixture
{
    [Test]
    public async Task Then_Without_Required_Fields_Bad_Request_Is_Returned()
    {
        // act
        var response = await Client.PostAsJsonAsync($"{RouteNames.EmployerProfile}/1/{RouteElements.EmployerProfileAddresses}", new {});
        var errors = await response.Content.ReadAsAsync<ValidationProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Should().NotBeNull();
        errors.Errors.Should().HaveCount(2);
        errors.Errors.Should().ContainKey(nameof(PostEmployerProfileAddressRequest.AddressLine1));
        errors.Errors.Should().ContainKey(nameof(PostEmployerProfileAddressRequest.Postcode));
    }
}