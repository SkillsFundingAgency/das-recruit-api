using System.ComponentModel.DataAnnotations;
using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Extensions;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.ApplicationReview;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.Controllers;

[Route("api/[controller]s/{id:guid}")]
[ApiController]
public class ApplicationReviewController([FromServices] IApplicationReviewsProvider provider,
    ILogger<ApplicationReviewController> logger) : ControllerBase
{
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