using System.Net;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.VacancyAnalytics;
using SFA.DAS.Recruit.Api.Models.Responses.VacancyAnalytics;

namespace SFA.DAS.Recruit.Api.Controllers;
[ApiController]
[Route($"{RouteNames.VacancyAnalytics}/{{vacancyReference:long}}")]
public class VacancyAnalyticsController(
    ILogger<VacancyAnalyticsController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(VacancyAnalyticsResponse), StatusCodes.Status200OK)]
    public async Task<IResult> GetOne(
        [FromRoute] long vacancyReference,
        [FromServices] IVacancyAnalyticsRepository repository,
        CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Recruit API: Received request to get vacancy analytics for vacancy reference: {VacancyReference}", vacancyReference);

            var result = await repository.GetOneAsync(vacancyReference, token);

            return result is null 
                ? TypedResults.NotFound() 
                : TypedResults.Ok(result.ToResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to get vacancy analytics : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(VacancyAnalyticsResponse), StatusCodes.Status201Created)]
    public async Task<IResult> PutOne(
        [FromRoute] long vacancyReference,
        [FromServices] IVacancyAnalyticsRepository repository,
        [FromBody] PutVacancyAnalyticsRequest request,
        CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Recruit API: Received request to create vacancy analytics for vacancy reference: {VacancyReference}", vacancyReference);

            var result = await repository.UpsertOneAsync(request.ToEntity(vacancyReference), token);

            return TypedResults.Created($"/{RouteNames.VacancyAnalytics}/{result.Entity.VacancyReference}", result.Entity.ToResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to create vacancy analytics : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }
}
