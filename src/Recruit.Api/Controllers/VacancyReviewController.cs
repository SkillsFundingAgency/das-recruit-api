using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Extensions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Data.VacancyReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.VacancyReview;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route($"{RouteNames.VacancyReviews}/{{id:guid}}")]
public class VacancyReviewController(ILogger<VacancyReviewController> logger): ControllerBase
{
    [HttpGet, Route($"~/{RouteNames.VacancyReviews}")]
    [ProducesResponseType(typeof(List<VacancyReview>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetMany(
        [FromServices] IVacancyReviewRepository repository,
        [FromQuery] List<ReviewStatus> reviewStatus,
        [FromQuery] DateTime? expiredAssignationDateTime,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetManyByStatusAndExpiredAssignationDateTime(reviewStatus, expiredAssignationDateTime, cancellationToken);

        return TypedResults.Ok(result.ToGetResponse());
    }

    [HttpGet]
    [ProducesResponseType(typeof(VacancyReview), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOne(
        [FromServices] IVacancyReviewRepository repository,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetOneAsync(id, cancellationToken);

        return result is null
            ? Results.NotFound()
            : TypedResults.Ok(result.ToGetResponse());
    }

    [HttpPut]
    [ProducesResponseType(typeof(VacancyReview), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(VacancyReview), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]   
    public async Task<IResult> PutOne(
        [FromServices] IVacancyReviewRepository repository,
        [FromServices] IUserRepository userRepository,
        [FromRoute] Guid id,
        [FromBody] PutVacancyReviewRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.SubmittedByUserEmail))
        {
            if (string.IsNullOrEmpty(request.SubmittedByUserId))
            {
                logger.LogError("No user information supplied for Vacancy {0}", request.VacancyReference);
                request.SubmittedByUserEmail = $"unknown-{request.VacancyReference}";
            }
            else
            {
                var submittedUser = await userRepository.FindByUserIdAsync(request.SubmittedByUserId, cancellationToken);

                if (submittedUser is null)
                {
                    logger.LogError("Unable to find user {0} for Vacancy {1}", request.SubmittedByUserId, request.VacancyReference);
                    request.SubmittedByUserEmail = $"unknown-{request.SubmittedByUserId}";
                }
                else
                {
                    request.SubmittedByUserEmail = submittedUser?.Email;    
                }    
            }
        }

        try
        {
            var result = await repository.UpsertOneAsync(request.ToDomain(id), cancellationToken);

            return result.Created
                ? TypedResults.Created($"/{RouteNames.VacancyReviews}/{result.Entity.Id}", result.Entity.ToPutResponse())
                : TypedResults.Ok(result.Entity.ToPutResponse());
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while updating VacancyReview");
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
        
    }
    
    [HttpPatch]
    [ProducesResponseType(typeof(VacancyReview), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> PatchOne(
        [FromServices] IVacancyReviewRepository repository,
        [FromRoute] Guid id,
        [FromBody] JsonPatchDocument<VacancyReview> patchRequest,
        CancellationToken cancellationToken)
    {
        var vacancyReview = await repository.GetOneAsync(id, cancellationToken);
        if (vacancyReview is null)
        {
            return Results.NotFound();
        }
        
        try
        {
            patchRequest.ThrowIfOperationsOn([
                nameof(VacancyReview.VacancyReference),
                nameof(VacancyReview.CreatedDate)
            ]);
            
            var patchDocument = patchRequest.ToDomain<VacancyReview, VacancyReviewEntity>();
            patchDocument.ApplyTo(vacancyReview);
        }
        catch (JsonPatchException ex)
        {
            return TypedResults.ValidationProblem(ex.ToProblemsDictionary());
        }

        await repository.UpsertOneAsync(vacancyReview, cancellationToken);
        return TypedResults.Ok(vacancyReview.ToPatchResponse());
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteOne(
        [FromServices] IVacancyReviewRepository repository,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        bool deleted = await repository.DeleteOneAsync(id, cancellationToken);

        return deleted
            ? Results.NoContent()
            : Results.NotFound();
    }
    
    [HttpGet, Route($"~/{RouteNames.Vacancies}/{{vacancyReference}}/reviews")]
    [ProducesResponseType(typeof(List<VacancyReview>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetManyByVacancyReference(
        [FromServices] IVacancyReviewRepository repository,
        [FromRoute] VacancyReference vacancyReference,
        [FromQuery] List<ReviewStatus>? status,
        [FromQuery] List<string>? manualOutcome,
        [FromQuery] bool? includeNoStatus,
        CancellationToken cancellationToken)
    {
        var statuses = status ?? new List<ReviewStatus>();
        var result = await repository.GetManyByVacancyReferenceAndStatus(vacancyReference, statuses, manualOutcome, includeNoStatus ?? false, cancellationToken);

        return TypedResults.Ok(result.ToGetResponse());
    }

    [HttpGet, Route($"~/{RouteNames.Account}/{{accountLegalEntityId}}/vacancyreviews")]
    [ProducesResponseType(typeof(List<VacancyReview>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetManyByAccountLegalEntityId(
        [FromServices] IVacancyReviewRepository repository,
        [FromRoute] long accountLegalEntityId,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetManyByAccountLegalEntityId(accountLegalEntityId, cancellationToken);

        return TypedResults.Ok(result.ToGetResponse());
    }

    [HttpGet, Route($"~/{RouteNames.Account}/{{accountLegalEntityId}}/vacancyreviews/count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetCountByAccountLegalEntityId(
        [FromServices] IVacancyReviewRepository repository,
        [FromRoute] long accountLegalEntityId,
        [FromQuery] List<ReviewStatus>? status,
        [FromQuery] List<string>? manualOutcome,
        [FromQuery] EmployerNameOption? employerNameOption,
        CancellationToken cancellationToken)
    {
        var statuses = status ?? [];
        var count = await repository.GetCountByAccountLegalEntityId(accountLegalEntityId, statuses, manualOutcome, employerNameOption, cancellationToken);

        return TypedResults.Ok(count);
    }

    [HttpGet, Route($"~/{RouteNames.VacancyReviews}/qa/dashboard")]
    [ProducesResponseType(typeof(QaDashboard), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> GetQaDashboard(
        [FromServices] IVacancyReviewRepository repository,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await repository.GetQaDashboard(cancellationToken);

            return TypedResults.Ok(result);
        }
        catch
        {
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet, Route("~/api/users/vacancyreviews")]
    [ProducesResponseType(typeof(List<VacancyReview>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetManyByUser(
        [FromServices] IVacancyReviewRepository repository,
        [FromQuery] string userId,
        [FromQuery] DateTime? assignationExpiry,
        [FromQuery] ReviewStatus? status,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetManyByReviewedByUserEmailAndAssignationExpiry(userId, assignationExpiry, status, cancellationToken);

        return TypedResults.Ok(result.ToGetResponse());
    }

    [HttpGet, Route("~/api/users/vacancyreviews/count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetCountByUser(
        [FromServices] IVacancyReviewRepository repository,
        [FromQuery] string userEmail,
        [FromQuery] bool? approvedFirstTime,
        [FromQuery] DateTime? assignationExpiry,
        CancellationToken cancellationToken)
    {
        var count = await repository.GetCountBySubmittedUserEmail(userEmail, approvedFirstTime, assignationExpiry, cancellationToken);

        return TypedResults.Ok(count);
    }
}