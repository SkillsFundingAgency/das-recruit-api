using System.ComponentModel.DataAnnotations;
using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Recruit.Api.Application.Providers;
using Recruit.Api.Domain.Enums;
using Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Extensions;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests;
using SFA.DAS.Recruit.Api.Models.Responses;

namespace SFA.DAS.Recruit.Api.Controllers;

[Route("api/[controller]s/{id:guid}")]
[ApiController]
public class ApplicationReviewController([FromServices] IApplicationReviewsProvider provider,
    ILogger<ApplicationReviewController> logger) : ControllerBase
{
    public record ApplicationReviewsResponse(PageInfo PageInfo, IEnumerable<ApplicationReview> ApplicationReviews);

    [HttpGet]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GetApplicationReviewResponse), StatusCodes.Status200OK)]
    public async Task<IResult> Get(
        [FromRoute] [Required] Guid id, 
        CancellationToken token)
    {
        try
        {
            logger.LogInformation("Recruit API: Received query to get application review by id : {Id}", id);

            var response = await provider.GetById(id, token);

            return response == null
                ? Results.NotFound()
                : TypedResults.Ok(response.ToGetResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to Get application review : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet]
    [Route("~/api/employer/{accountId:long}/[controller]s")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationReviewsResponse), StatusCodes.Status200OK)]
    public async Task<IResult> GetAllByAccountId(
        [FromRoute] [Required] long accountId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortColumn = "CreatedDate",
        [FromQuery] bool isAscending = false,
        CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Recruit API: Received query to get all application reviews by account id : {AccountId}", accountId);

            var response = await provider.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

            var mappedResults = response.Items.Select(app => app.ToApplicationReview());

            return TypedResults.Ok(new ApplicationReviewsResponse(response.ToPageInfo(), mappedResults));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to Get all application reviews by account Id : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet]
    [Route("~/api/employer/{accountId:long}/[controller]s/dashboard")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(DashboardModel), StatusCodes.Status200OK)]
    public async Task<IResult> GetDashboardCountByAccountId(
        [FromRoute][Required] long accountId,
        [FromQuery][Required] ApplicationStatus status,
        CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Recruit API: Received query to get dashboard stats by account id : {AccountId}", accountId);

            var response = await provider.GetCountByAccountId(accountId, status, token);

            return TypedResults.Ok(response);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to get dashboard stats by account id : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet]
    [Route("~/api/provider/{ukprn:int}/[controller]s")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationReviewsResponse), StatusCodes.Status200OK)]
    public async Task<IResult> GetAllByUkprn(
        [FromRoute] [Required] int ukprn,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortColumn = "CreatedDate",
        [FromQuery] bool isAscending = false,
        CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Recruit API: Received query to get all application reviews by ukprn : {ukprn}", ukprn);

            var response = await provider.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token);

            var mappedResults = response.Items.Select(app => app.ToApplicationReview());

            return TypedResults.Ok(new ApplicationReviewsResponse(response.ToPageInfo(), mappedResults));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to Get all application reviews by ukprn : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet]
    [Route("~/api/provider/{ukprn:int}/[controller]s/dashboard")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(DashboardModel), StatusCodes.Status200OK)]
    public async Task<IResult> GetDashboardCountByUkprn(
        [FromRoute][Required] int ukprn,
        [FromQuery][Required] ApplicationStatus status,
        CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Recruit API: Received query to get dashboard stats by ukprn : {ukprn}", ukprn);

            var response = await provider.GetCountByUkprn(ukprn, status, token);

            return TypedResults.Ok(response);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to Get dashboard stats by ukprn : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }
    
    [HttpPut]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(PutApplicationReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PutApplicationReviewResponse), StatusCodes.Status201Created)]
    public async Task<IResult> Put(
        [FromRoute] [Required] Guid id,
        [FromBody] [Required] PutApplicationReviewRequest request,
        [FromServices] IValidator<PutApplicationReviewRequest> requestValidator,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Recruit API: Received command to put application review for Id : {id}", id);
        try
        {
            var validationResult = await requestValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            var result = await provider.Upsert(request.ToEntity(id), cancellationToken);
            return result.Created
                ? TypedResults.Created($"{result.Entity.Id}", result.Entity.ToPutResponse())
                : TypedResults.Ok(result.Entity.ToPutResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Upsert ApplicationReview: An error occurred");
            return TypedResults.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpPatch]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(PatchApplicationReviewResponse), StatusCodes.Status200OK)]
    public async Task<IResult> Patch(
        [FromRoute] Guid id,
        [FromBody] JsonPatchDocument<ApplicationReview> patchRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Recruit API: Received command to patch application review for Id : {id}", id);

            var applicationReview = await provider.GetById(id, cancellationToken);
            if (applicationReview is null)
            {
                return TypedResults.NotFound();
            }
            
            var entityPatchDocument = patchRequest.ToEntity();
            entityPatchDocument.ApplyTo(applicationReview);
            
            await provider.Update(applicationReview, cancellationToken);
            return TypedResults.Ok(applicationReview.ToPatchResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to update application review : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }
}