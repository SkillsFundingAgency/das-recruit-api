using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.Controllers
{
    [Route($"{RouteNames.Employer}/{{accountId:long}}/")]
    public class EmployerAccountController([FromServices] IApplicationReviewsProvider applicationReviewsProvider,
        [FromServices] IVacancyProvider vacancyProvider,
        [FromServices] IAlertsProvider alertsProvider,
        ILogger<ApplicationReviewController> logger) : ControllerBase
    {
        [HttpGet]
        [Route("applicationReviews")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApplicationReviewsResponse), StatusCodes.Status200OK)]
        public async Task<IResult> GetAllByAccountId(
            [FromRoute][Required] long accountId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
            [FromQuery] bool isAscending = false,
            CancellationToken token = default)
        {
            try
            {
                logger.LogInformation("Recruit API: Received query to get all application reviews by account id : {AccountId}", accountId);

                var response = await applicationReviewsProvider.GetPagedAccountIdAsync(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

                var mappedResults = response.Items.Select(app => app.ToGetResponse());

                return TypedResults.Ok(new ApplicationReviewsResponse(response.ToPageInfo(), mappedResults));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unable to Get all application reviews by account Id : An error occurred");
                return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("applicationReviews/dashboard")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(EmployerDashboardModel), StatusCodes.Status200OK)]
        public async Task<IResult> GetDashboardCountByAccountId(
            [FromRoute][Required] long accountId,
            [FromQuery] string? userId = null,
            CancellationToken token = default)
        {
            try
            {
                logger.LogInformation("Recruit API: Received query to get dashboard stats by account id : {AccountId}", accountId);

                var applicationReviewsResponse = await applicationReviewsProvider.GetCountByAccountId(accountId, token);
                var vacancyResponse = await vacancyProvider.GetCountByAccountId(accountId, token);

                if (string.IsNullOrEmpty(userId))
                    return TypedResults.Ok(new EmployerDashboardModel(applicationReviewsResponse,
                        vacancyResponse));


                var employerRevokedTransferredVacanciesAlert = await alertsProvider.GetEmployerTransferredVacanciesAlertByAccountId(accountId, userId, TransferReason.EmployerRevokedPermission, token);
                var blockedProviderTransferredVacanciesAlert = await alertsProvider.GetEmployerTransferredVacanciesAlertByAccountId(accountId, userId, TransferReason.BlockedByQa, token);
                var blockedProviderAlert = await alertsProvider.GetBlockedProviderAlertCountByAccountId(accountId, userId, token);
                var withdrawnByQaVacanciesAlert = await alertsProvider.GetWithDrawnByQaAlertByAccountId(accountId, userId, token);
                var dashboardModel = new EmployerDashboardModel(applicationReviewsResponse,
                    vacancyResponse) {
                    EmployerRevokedTransferredVacanciesAlert = employerRevokedTransferredVacanciesAlert,
                    BlockedProviderTransferredVacanciesAlert = blockedProviderTransferredVacanciesAlert,
                    WithDrawnByQaVacanciesAlert = withdrawnByQaVacanciesAlert,
                    BlockedProviderAlert = blockedProviderAlert
                };

                return TypedResults.Ok(dashboardModel);

            }
            catch (Exception e)
            {
                logger.LogError(e, "Unable to get dashboard stats by account id : An error occurred");
                return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("applicationReviews/dashboard/vacancies")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(VacancyDashboardResponse), StatusCodes.Status200OK)]
        public async Task<IResult> GetDashboardVacanciesCountByAccountId(
            [FromRoute][Required] long accountId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 25,
            [FromQuery] string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
            [FromQuery] bool isAscending = false,
            [FromQuery] List<ApplicationReviewStatus>? status = null,
            CancellationToken token = default)
        {
            try
            {
                logger.LogInformation("Recruit API: Received query to get dashboard vacancy count by account id : {AccountId}", accountId);

                var response = status != null && status.Contains(ApplicationReviewStatus.AllShared)
                    ? await applicationReviewsProvider.GetPagedAllSharedByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending,
                        token)
                    : await applicationReviewsProvider.GetPagedByAccountAndStatusAsync(accountId, pageNumber, pageSize, sortColumn, isAscending, status,
                        token);

                return TypedResults.Ok(new VacancyDashboardResponse(response.ToPageInfo(), response.Items));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unable to get dashboard vacancy count by account id : An error occurred");
                return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("applicationReviews/count")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApplicationReviewsStats), StatusCodes.Status200OK)]
        public async Task<IResult> GetCountByVacancyReferences(
            [FromRoute][Required] long accountId,
            [FromQuery] string? applicationSharedFilteringStatus,
            [FromBody][Required] List<long> vacancyReferences,
            CancellationToken token = default)
        {
            try
            {
                logger.LogInformation("Recruit API: Received query to get vacancy references count by account id : {AccountId}", accountId);

                ApplicationReviewStatus? status = null;
                if (Enum.TryParse<ApplicationReviewStatus>(applicationSharedFilteringStatus, true, out var parsedStatus))
                {
                    status = parsedStatus;
                }
                
                var response = await applicationReviewsProvider.GetVacancyReferencesCountByAccountId(accountId, vacancyReferences, status, token);

                return TypedResults.Ok(response);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unable to get vacancy references count by account Id : An error occurred");
                return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
