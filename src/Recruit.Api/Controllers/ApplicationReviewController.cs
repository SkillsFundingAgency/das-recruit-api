using System.ComponentModel.DataAnnotations;
using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.ApplicationReview;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController]
public class ApplicationReviewController([FromServices] IApplicationReviewsProvider provider,
    ILogger<ApplicationReviewController> logger) : ControllerBase
{
    [HttpGet]
    [Route($"{RouteNames.ApplicationReview}/{{applicationId:guid}}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GetApplicationReviewResponse), StatusCodes.Status200OK)]
    public async Task<IResult> Get(
        [FromRoute] [Required] Guid applicationId, 
        CancellationToken token)
    {
        try
        {
            logger.LogInformation("Recruit API: Received query to get application review by id : {Id}", applicationId);

            var response = await provider.GetById(applicationId, token);

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
    [Route($"{RouteNames.ApplicationReview}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GetApplicationReviewResponse), StatusCodes.Status200OK)]
    public async Task<IResult> GetByApplicationId(
        [FromQuery] [Required] Guid applicationId, 
        CancellationToken token)
    {
        try
        {
            logger.LogInformation("Recruit API: Received query to get application review by application id : {Id}", applicationId);

            var response = await provider.GetByApplicationId(applicationId, token);

            return response == null
                ? Results.NotFound()
                : TypedResults.Ok(response.ToGetResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to Get application review by application id : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet]
    [Route($"~/{RouteNames.ApplicationReview}/{{vacancyReference}}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(List<GetApplicationReviewResponse>), StatusCodes.Status200OK)]
    public async Task<IResult> GetManyByVacancyReference(
        [FromRoute][Required] long vacancyReference,
        CancellationToken token)
    {
        try
        {
            logger.LogInformation("Recruit API: Received query to get all application reviews by vacancyReference : {vacancyReference}", vacancyReference);

            var response = await provider.GetAllByVacancyReference(vacancyReference, token);

            return response is null or { Count: 0} 
                ? Results.NotFound() 
                : TypedResults.Ok(response.ToGetResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to Get application reviews by vacancyReference : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost]
    [Route($"~/{RouteNames.ApplicationReview}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(List<GetApplicationReviewResponse>), StatusCodes.Status200OK)]
    public async Task<IResult> GetManyByIds(
        [FromBody][Required] List<Guid> ids,
        CancellationToken token)
    {
        try
        {
            logger.LogInformation("Recruit API: Received query to get all application reviews by ids");

            var response = await provider.GetAllByIdAsync(ids, token);

            return TypedResults.Ok(response.ToGetResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to Get application reviews by ids : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet]
    [Route($"~/{RouteNames.ApplicationReview}/{{vacancyReference}}/Candidate/{{candidateId:guid}}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GetApplicationReviewResponse), StatusCodes.Status200OK)]
    public async Task<IResult> GetOneByVacancyReferenceAndCandidateId(
        [FromRoute][Required] long vacancyReference,
        [FromRoute][Required] Guid candidateId,
        CancellationToken token)
    {
        try
        {
            logger.LogInformation("Recruit API: Received query to get all application reviews by vacancyReference and candidateId : {vacancyReference}", vacancyReference);

            var response = await provider.GetByVacancyReferenceAndCandidateId(vacancyReference, candidateId, token);

            return response is null
                ? Results.NotFound()
                : TypedResults.Ok(response.ToGetResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to Get application reviews by vacancyReference and candidateId : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet]
    [Route($"~/{RouteNames.ApplicationReview}/{{vacancyReference}}/count")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApplicationReviewsStats), StatusCodes.Status200OK)]
    public async Task<IResult> GetStatusCountByVacancyReference(
        [FromRoute][Required] long vacancyReference,
        CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Recruit API: Received query to get application reviews status count by vacancyReference : {vacancyReference}", vacancyReference);

            var response = await provider.GetStatusCountByVacancyReference(vacancyReference, token);

            return TypedResults.Ok(response);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to Get get application reviews status count by vacancyReference : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet]
    [Route($"~/{RouteNames.ApplicationReview}/{{vacancyReference}}/temp-status/{{status}}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(List<GetApplicationReviewResponse>), StatusCodes.Status200OK)]
    public async Task<IResult> GetManyByVacancyReferenceAndTempStatus(
        [FromRoute][Required] long vacancyReference,
        [FromRoute][Required] ApplicationReviewStatus status,
        CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Recruit API: Received query to get all application reviews by vacancyReference and status : {vacancyReference}", vacancyReference);

            var response = await provider.GetAllByVacancyReferenceAndTempStatus(vacancyReference, status, token);

            return TypedResults.Ok(response.ToGetResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to Get application reviews by vacancyReference and status : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpPut]
    [Route($"{RouteNames.ApplicationReview}/{{applicationId:guid}}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(PutApplicationReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PutApplicationReviewResponse), StatusCodes.Status201Created)]
    public async Task<IResult> Put(
        [FromRoute] [Required] Guid applicationId,
        [FromBody] [Required] PutApplicationReviewRequest request,
        [FromServices] IValidator<PutApplicationReviewRequest> requestValidator,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Recruit API: Received command to put application review for Id : {id}", applicationId);
        try
        {
            var validationResult = await requestValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            var result = await provider.Upsert(request.ToEntity(applicationId), cancellationToken);
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
    [Route($"{RouteNames.ApplicationReview}/{{applicationId:guid}}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(PatchApplicationReviewResponse), StatusCodes.Status200OK)]
    public async Task<IResult> Patch(
        [FromRoute] Guid applicationId,
        [FromBody] JsonPatchDocument<ApplicationReview> patchRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Recruit API: Received command to patch application review for Id : {id}", applicationId);

            var applicationReview = await provider.GetById(applicationId, cancellationToken);
            if (applicationReview is null)
            {
                applicationReview = await provider.GetByApplicationId(applicationId, cancellationToken);
                if (applicationReview is null)
                {
                    return TypedResults.NotFound();    
                }
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