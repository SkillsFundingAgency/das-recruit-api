using System.Net;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.Report;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController]
[Route($"{RouteNames.Reports}")]
public class ReportController(ILogger<ReportController> logger) : ControllerBase
{
    [HttpGet]
    [Route($"{{reportId:guid}}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Report), StatusCodes.Status200OK)]
    public async Task<IResult> GetOne(
        [FromServices] IReportRepository reportRepository,
        [FromRoute] Guid reportId,
        CancellationToken token)
    {
        try
        {
            logger.LogInformation("Recruit API: Received request to get report for report Id: {ReportId}", reportId);

            var reports = await reportRepository.GetOneAsync(reportId, token);

            return reports == null 
                ? TypedResults.NotFound() 
                : TypedResults.Ok(reports.ToResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to get report : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet]
    [Route($"{{ukprn:int}}/{RouteElements.Provider}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<Report>), StatusCodes.Status200OK)]
    public async Task<IResult> GetByUkprn(
        [FromServices] IReportRepository reportRepository,
        [FromRoute] int ukprn,
        CancellationToken token)
    {
        try
        {
            logger.LogInformation("Recruit API: Received request to get reports for ukprn: {Ukprn}", ukprn);
            
            var reports = await reportRepository.GetManyByUkprn(ukprn, token);

            return TypedResults.Ok(reports.Select(r => r.ToResponse()).ToList());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to get reports : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet]
    [Route($"{RouteElements.Qa}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Report), StatusCodes.Status200OK)]
    public async Task<IResult> GetMany(
        [FromServices] IReportRepository reportRepository,
        CancellationToken token)
    {
        try
        {
            logger.LogInformation("Recruit API: Received request to get reports for Qa");

            var reports = await reportRepository.GetMany(token);

            return TypedResults.Ok(reports.Select(r => r.ToResponse()).ToList());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to get reports for Qa : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }
    
    [HttpGet]
    [Route($"generate/{{reportId:guid}}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<ApplicationReviewReport>), StatusCodes.Status200OK)]
    public async Task<IResult> Generate(
        [FromServices] IReportRepository reportRepository,
        [FromRoute] Guid reportId,
        CancellationToken token)
    {
        try
        {
            logger.LogInformation("Recruit API: Received request to generate report for report Id: {ReportId}", reportId);
            
            var reports = await reportRepository.Generate(reportId, token);

            return TypedResults.Ok(reports.ToGetResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to generate report : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Report), StatusCodes.Status201Created)]
    public async Task<IResult> Create(
        [FromServices] IReportRepository reportRepository,
        [FromBody] PostReportRequest request,
        CancellationToken token)
    {
        try
        {
            logger.LogInformation("Recruit API: Received request to create report for user Id: {UserId}", request.UserId);

            var result = await reportRepository.UpsertOneAsync(request.ToEntity(), token);

            return TypedResults.Created($"/{RouteNames.Reports}/{result.Entity.Id}", result.Entity.ToResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to create report : An error occurred");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }
}