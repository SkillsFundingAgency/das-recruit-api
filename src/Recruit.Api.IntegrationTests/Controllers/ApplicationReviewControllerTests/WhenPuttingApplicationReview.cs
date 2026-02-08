using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Models.Requests.ApplicationReview;
using SFA.DAS.Recruit.Api.Testing.Http;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.ApplicationReviewControllerTests;

public class WhenPuttingApplicationReview : BaseFixture
{
    [Test, MoqAutoData]
    public async Task Then_Without_Required_Fields_Bad_Request_Is_Returned(Guid id, PutApplicationReviewRequest request)
    {
        // arrange
        request.Ukprn = 0;
        request.AccountId = 0;
        request.CandidateId = Guid.Empty;
        request.VacancyReference = 0;
        request.AccountLegalEntityId = 0;
        
        // act
        var response = await Client.PutAsJsonAsync($"{RouteNames.ApplicationReview}/{id}", request);
        var errors = await response.Content.ReadAsAsync<ValidationProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Should().NotBeNull();
        errors.Errors.Should().HaveCount(5);
        errors.Errors.Should().ContainKey(nameof(PutApplicationReviewRequest.Ukprn));
        errors.Errors.Should().ContainKey(nameof(PutApplicationReviewRequest.AccountId));
        errors.Errors.Should().ContainKey(nameof(PutApplicationReviewRequest.CandidateId));
        errors.Errors.Should().ContainKey(nameof(PutApplicationReviewRequest.VacancyReference));
        errors.Errors.Should().ContainKey(nameof(PutApplicationReviewRequest.AccountLegalEntityId));
    }
}