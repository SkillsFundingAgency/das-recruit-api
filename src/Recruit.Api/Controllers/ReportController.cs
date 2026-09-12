using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.Report;
using SFA.DAS.Recruit.Api.Models.Responses.Report;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController]
[Route($"{RouteNames.Reports}")]
public class ReportController(ILogger<ReportController> logger, IBlobStorageService blobStorageService)
    : ControllerBase
{
    [HttpGet]
    [Route("{reportId:guid}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetApplicationReviewReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GetQaReportResponse), StatusCodes.Status200OK)]
    public async Task<IResult> GetOne(
        [FromServices] IReportRepository reportRepository,
        [FromRoute, Required] Guid reportId,
        CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Recruit API: Received request to get report for report Id: {ReportId}", reportId);

            var reportEntity = await reportRepository.GetOneAsync(reportId, token);
            if (reportEntity == null) return TypedResults.NotFound();

            if (reportEntity.BlobStorageId.HasValue)
            {
                var json = await blobStorageService.DownloadAsync(reportEntity.BlobStorageId.Value, token);
                if (reportEntity.Type == ReportType.QaApplications)
                {
                    var qaResponse = JsonSerializer.Deserialize<GetQaReportResponse>(json, JsonConfig.Options);
                    return TypedResults.Ok(qaResponse);
                }
                var response = JsonSerializer.Deserialize<GetApplicationReviewReportResponse>(json, JsonConfig.Options);
                return TypedResults.Ok(response);
            }

            // Fallback: report predates blob storage — generate on the fly
            if (reportEntity.Type == ReportType.QaApplications)
            {
                var qaReports = await reportRepository.GenerateQa(reportId, token);
                return TypedResults.Ok(qaReports.ToGetQaResponse());
            }

            var reports = await reportRepository.Generate(reportId, token);
            return TypedResults.Ok(reports.ToGetResponse());
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
        [FromRoute, Required] int ukprn,
        CancellationToken token = default)
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Report), StatusCodes.Status200OK)]
    public async Task<IResult> GetMany(
        [FromServices] IReportRepository reportRepository,
        [FromQuery] ReportOwnerType ownerType,
        CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Recruit API: Received request to get reports for {OwnerType}", ownerType);

            var reports = await reportRepository.GetMany(ownerType, token);

            return TypedResults.Ok(reports.Select(r => r.ToResponse()).ToList());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to get reports for {OwnerType} : An error occurred", ownerType);
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }
    
    [HttpGet]
    [Route("generate/{reportId:guid}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IResult> Generate(
        [FromServices] IReportRepository reportRepository,
        [FromRoute, Required] Guid reportId,
        CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Recruit API: Received request to generate report for report Id: {ReportId}", reportId);

            var reports = await reportRepository.Generate(reportId, token);
            var json = JsonSerializer.Serialize(reports.ToGetResponse(), JsonConfig.Options);
            var blobId = await blobStorageService.UploadAsync(json, token);

            await reportRepository.SetBlobStorageIdAsync(reportId, blobId, token);
            await reportRepository.IncrementReportDownloadCountAsync(reportId, token);
            await reportRepository.SetStatusAsync(reportId, ReportStatus.Generated, token);
            
            return TypedResults.Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to generate report : An error occurred");
            await SetReportStatusToFailed(reportRepository, reportId, token);
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet]
    [Route($"generate-qa/{{reportId:guid}}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetQaReportResponse), StatusCodes.Status200OK)]
    public async Task<IResult> GenerateQa(
        [FromServices] IReportRepository reportRepository,
        [FromRoute, Required] Guid reportId,
        CancellationToken token = default)
    {
        try
        {
            logger.LogInformation("Recruit API: Received request to generate QA report for report Id: {ReportId}", reportId);

            var reports = await reportRepository.GenerateQa(reportId, token);

            await reportRepository.IncrementReportDownloadCountAsync(reportId, token);
            await reportRepository.SetStatusAsync(reportId, ReportStatus.Generated, token);

            return TypedResults.Ok(reports.ToGetQaResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to generate QA report : An error occurred");
            await SetReportStatusToFailed(reportRepository, reportId, token);
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
        CancellationToken token = default)
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

    private async Task SetReportStatusToFailed(IReportRepository reportRepository, Guid reportId, CancellationToken token)
    {
        try
        {
            await reportRepository.SetStatusAsync(reportId, ReportStatus.Failed, token);
        }
        catch (Exception statusEx)
        {
            logger.LogError(statusEx, "Unable to set report status to Failed for report Id: {ReportId}", reportId);
        }
    }
}