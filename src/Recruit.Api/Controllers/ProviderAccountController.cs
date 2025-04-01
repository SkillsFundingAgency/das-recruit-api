﻿using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Recruit.Api.Application.Providers;
using Recruit.Api.Domain.Enums;
using Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Extensions;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Controllers
{
    [Route("api/provider/{ukprn:int}/")]
    public class ProviderAccountController([FromServices]
    IApplicationReviewsProvider provider,
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
        [Route("applicationReviews/dashboard")]
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

        [HttpPost]
        [Route("applicationReviews/count")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApplicationReviewsStats), StatusCodes.Status200OK)]
        public async Task<IResult> GetCountByVacancyReferences(
            [FromRoute][Required] int ukprn,
            [FromQuery][Required] ApplicationStatus status,
            [FromBody][Required] List<long> vacancyReferences,
            CancellationToken token = default)
        {
            try
            {
                logger.LogInformation("Recruit API: Received query to get vacancy references count by ukprn : {ukprn}", ukprn);

                var response = await provider.GetVacancyReferencesCountByUkprn(ukprn, vacancyReferences, status, token);

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
