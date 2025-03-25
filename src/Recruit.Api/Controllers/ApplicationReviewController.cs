using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Recruit.Api.Application.Models.ApplicationReview;
using Recruit.Api.Application.Providers;
using Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Extensions;
using SFA.DAS.Recruit.Api.Models.Requests;
using SFA.DAS.Recruit.Api.Models.Responses;

namespace SFA.DAS.Recruit.Api.Controllers;

[Route("api/[controller]s/{id:guid}")]
[ApiController]
public class ApplicationReviewController([FromServices] IApplicationReviewsProvider provider,
    ILogger<ApplicationReviewController> logger) : ControllerBase
{
    public record ApplicationReviewsResponse(PageInfo PageInfo, IEnumerable<ApplicationReviewResponse> ApplicationReviews);

    [HttpGet]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApplicationReviewResponse), StatusCodes.Status200OK)]
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
                : TypedResults.Ok(response.ToResponse());
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

            var mappedResults = response.Items.Select(app => app.ToResponse());

            return TypedResults.Ok(new ApplicationReviewsResponse(response.ToPageInfo(), mappedResults));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to Get all application reviews by account Id : An error occurred");
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

            var mappedResults = response.Items.Select(app => app.ToResponse());

            return TypedResults.Ok(new ApplicationReviewsResponse(response.ToPageInfo(), mappedResults));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to Get all application reviews by ukprn : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpPut]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationReviewResponse), StatusCodes.Status200OK)]
    public async Task<IResult> Put(
        [FromRoute] [Required] Guid id,
        [FromBody] [Required] ApplicationReviewRequest request,
        CancellationToken token)
    {
        try
        {
            logger.LogInformation("Recruit API: Received command to put application review for Id : {id}", id);

            var response = await provider.Upsert(request.ToEntity(), token);

            if (response.Item2)
            {
                return TypedResults.Created($"{response.Item1.Id}", response.Item1.ToResponse());
            }

            return TypedResults.Ok(response.Item1.ToResponse());
        }
        catch (ValidationException e)
        {
            return Results.BadRequest(e.ValidationResult.ErrorMessage);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Upsert ApplicationReview : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpPatch]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationReviewResponse), StatusCodes.Status200OK)]
    public async Task<IResult> Patch([FromRoute] Guid id, [FromBody] JsonPatchDocument<PatchApplicationReview> applicationRequest)
    {
        try
        {
            logger.LogInformation("Recruit API: Received command to patch application review for Id : {id}", id);

            var result = await provider.Update(new PatchApplication
            {
                Id = id,
                Patch = applicationRequest
            });

            return result == null 
                ? Results.NotFound() 
                : TypedResults.Ok(result.ToResponse());
        }
        catch (ValidationException e)
        {
            return Results.BadRequest(e.ValidationResult.ErrorMessage);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to update application review : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }
}