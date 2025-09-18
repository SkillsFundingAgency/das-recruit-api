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
    [Route($"{RouteNames.Provider}/{{ukprn:int}}/")]
    public class ProviderAccountController([FromServices] IApplicationReviewsProvider applicationReviewsProvider,
        [FromServices] IVacancyProvider vacancyProvider,
        ILogger<ApplicationReviewController> logger) : ControllerBase
    {
        [HttpGet]
        [Route("applicationReviews")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApplicationReviewsResponse), StatusCodes.Status200OK)]
        public async Task<IResult> GetAllByUkprn(
         [FromRoute][Required] int ukprn,
         [FromQuery] int pageNumber = 1,
         [FromQuery] int pageSize = 10,
         [FromQuery] string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
         [FromQuery] bool isAscending = false,
         CancellationToken token = default)
        {
            try
            {
                logger.LogInformation("Recruit API: Received query to get all application reviews by ukprn : {ukprn}", ukprn);

                var response = await applicationReviewsProvider.GetPagedUkprnAsync(ukprn, pageNumber, pageSize, sortColumn, isAscending, token);

                var mappedResults = response.Items.Select(app => app.ToGetResponse());

                return TypedResults.Ok(new ApplicationReviewsResponse(response.ToPageInfo(), mappedResults));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unable to Get all application reviews by ukprn : An error occurred");
                return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("applicationReviews/dashboard")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(DashboardModel), StatusCodes.Status200OK)]
        public async Task<IResult> GetDashboardCountByUkprn(
            [FromRoute][Required] int ukprn,
            CancellationToken token = default)
        {
            try
            {
                logger.LogInformation("Recruit API: Received query to get dashboard stats by ukprn : {ukprn}", ukprn);

                var applicationReviewsResponse = await applicationReviewsProvider.GetCountByUkprn(ukprn, token);
                var vacancyResponse = await vacancyProvider.GetCountByUkprn(ukprn, token);
                var dashboardModel = DashboardModel.MapToDashboardModel(applicationReviewsResponse, vacancyResponse);

                return TypedResults.Ok(dashboardModel);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unable to Get dashboard stats by ukprn : An error occurred");
                return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("applicationReviews/dashboard/vacancies")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(VacancyDashboardResponse), StatusCodes.Status200OK)]
        public async Task<IResult> GetDashboardVacanciesCountByAccountId(
            [FromRoute][Required] int ukprn,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 25,
            [FromQuery] string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
            [FromQuery] bool isAscending = false,
            [FromQuery] List<ApplicationReviewStatus>? status = null,
            CancellationToken token = default)
        {
            try
            {
                logger.LogInformation("Recruit API: Received query to get dashboard vacancy count by ukprn : {Ukprn}", ukprn);

                var response = await applicationReviewsProvider.GetPagedByUkprnAndStatusAsync(ukprn, pageNumber, pageSize, sortColumn, isAscending, status, token);

                return TypedResults.Ok(new VacancyDashboardResponse(response.ToPageInfo(), response.Items));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unable to get dashboard vacancy count by ukprn : An error occurred");
                return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("applicationReviews/count")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApplicationReviewsStats), StatusCodes.Status200OK)]
        public async Task<IResult> GetCountByVacancyReferences(
            [FromRoute][Required] int ukprn,
            [FromBody][Required] List<long> vacancyReferences,
            CancellationToken token = default)
        {
            try
            {
                logger.LogInformation("Recruit API: Received query to get vacancy references count by ukprn : {ukprn}", ukprn);

                var response = await applicationReviewsProvider.GetVacancyReferencesCountByUkprn(ukprn, vacancyReferences, token);

                return TypedResults.Ok(response);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unable to get vacancy references count by ukprn : An error occurred");
                return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
