﻿using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Extensions;
using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Models.Responses;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route($"{RouteNames.Vacancies}")]
public class VacancyController : Controller
{
    [HttpGet, Route("{vacancyId:guid}")]
    [ProducesResponseType(typeof(Vacancy), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOne(
        [FromServices] IVacancyRepository repository,
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetOneAsync(vacancyId, cancellationToken);

        return result is null
            ? Results.NotFound()
            : TypedResults.Ok(result.ToGetResponse());
    }

    [HttpGet, Route("{vacancyReference:long}/closed")]
    [ProducesResponseType(typeof(Vacancy), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOneClosedVacancyByVacancyReference(
        [FromServices] IVacancyRepository repository,
        [FromRoute] long vacancyReference,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetOneClosedVacancyByVacancyReference(vacancyReference, cancellationToken);

        return result is null
            ? Results.NotFound()
            : TypedResults.Ok(result.ToGetResponse());
    }

    [HttpPost, Route("closed")]
    [ProducesResponseType(typeof(Vacancy), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetManyClosedVacanciesByVacancyReferences(
        [FromServices] IVacancyRepository repository,
        [FromBody] PostClosedVacanciesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetManyClosedVacanciesByVacancyReferences(request.VacancyReferences, cancellationToken);

        return TypedResults.Ok(result
            .Select(v => v.ToGetResponse())
            .ToList());
    }

    [HttpGet, Route($"~/{RouteNames.Account}/{{accountId:long}}/{RouteElements.Vacancies}")]
    [ProducesResponseType(typeof(PagedResponse<VacancySummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetManyByAccountId(
        [FromServices] IVacancyProvider vacancyProvider,
        [FromServices] IApplicationReviewsProvider applicationReviewsProvider,
        [FromRoute] long accountId,
        PagingParams pagingParams,
        SortingParams<VacancySortColumn> sortingParams,
        FilteringParams filterParams,
        CancellationToken cancellationToken = default)
    {
        ushort page = pagingParams.Page ?? 1;
        ushort pageSize = pagingParams.PageSize ?? 25;
        var sortOrder = sortingParams.SortOrder ?? SortOrder.Asc;
        string searchTerm = filterParams.SearchTerm ?? string.Empty;

        var vacancies = await vacancyProvider.GetPagedVacancyByAccountId(
            accountId,
            page,
            pageSize,
            sortingParams.SortColumn.Resolve(),
            sortOrder,
            filterParams.FilterBy,
            searchTerm,
            cancellationToken);

        if (vacancies.TotalCount == 0)
        {
            return TypedResults.Ok(vacancies.ToPagedResponse(x => x.ToSummary()));
        }

        ApplicationReviewStatus? sharedFilter = filterParams.FilterBy switch {
            FilteringOptions.NewSharedApplications => ApplicationReviewStatus.Shared,
            FilteringOptions.AllSharedApplications => ApplicationReviewStatus.AllShared,
            _ => null,
        };

        // Collect distinct vacancy references
        var vacancyRefs = vacancies.Items
            .Where(x => x.VacancyReference.HasValue)
            .Select(x => x.VacancyReference!.Value)
            .Distinct()
            .ToList();

        var applicationReviewStats = await applicationReviewsProvider
            .GetVacancyReferencesCountByAccountId(accountId, vacancyRefs, sharedFilter, cancellationToken);

        var applicationReviewStatsDict = applicationReviewStats.ToDictionary(x => x.VacancyReference);

        var vacancySummaries = vacancies.ToPagedResponse(v =>
        {
            var summary = v.ToSummary();

            if (summary.VacancyReference.HasValue &&
                applicationReviewStatsDict.TryGetValue(summary.VacancyReference.Value, out var review))
            {
                summary.NoOfSuccessfulApplications = review.SuccessfulApplications;
                summary.NoOfUnsuccessfulApplications = review.UnsuccessfulApplications;
                summary.NoOfNewApplications = review.NewApplications;
                summary.NoOfAllSharedApplications =
                    filterParams.FilterBy == FilteringOptions.AllSharedApplications
                        ? review.Applications
                        : review.SharedApplications;
                summary.NoOfSharedApplications = review.SharedApplications;
                summary.NoOfEmployerReviewedApplications = review.EmployerReviewedApplications;
            }

            return summary;
        });

        return TypedResults.Ok(vacancySummaries);
    }

    [HttpGet, Route($"~/{RouteNames.Provider}/{{ukprn:int}}/{RouteElements.Vacancies}")]
    [ProducesResponseType(typeof(PagedResponse<VacancySummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetManyByUkprnId(
        [FromServices] IVacancyProvider vacancyProvider,
        [FromServices] IApplicationReviewsProvider applicationReviewsProvider,
        [FromRoute] int ukprn,
        PagingParams pagingParams,
        SortingParams<VacancySortColumn> sortingParams,
        FilteringParams filterParams,
        CancellationToken cancellationToken = default)
    {
        ushort page = pagingParams.Page ?? 1;
        ushort pageSize = pagingParams.PageSize ?? 25;
        var sortOrder = sortingParams.SortOrder ?? SortOrder.Asc;
        string searchTerm = filterParams.SearchTerm ?? string.Empty;

        var vacancies = await vacancyProvider.GetPagedVacancyByUkprn(
            ukprn,
            page,
            pageSize,
            sortingParams.SortColumn.Resolve(),
            sortOrder,
            filterParams.FilterBy,
            searchTerm,
            cancellationToken);

        if (vacancies.TotalCount == 0)
        {
            return TypedResults.Ok(vacancies.ToPagedResponse(x => x.ToSummary()));
        }

        // Collect distinct vacancy references
        var vacancyRefs = vacancies.Items
            .Where(x => x.VacancyReference is not null)
            .Select(x => x.VacancyReference!.Value)
            .Distinct()
            .ToList();

        var applicationReviewStats = await applicationReviewsProvider
            .GetVacancyReferencesCountByUkprn(ukprn, vacancyRefs, cancellationToken);

        var applicationReviewStatsDict = applicationReviewStats.ToDictionary(x => x.VacancyReference);

        var vacancySummaries = vacancies.ToPagedResponse(v =>
        {
            var summary = v.ToSummary();

            if (summary.VacancyReference.HasValue &&
                applicationReviewStatsDict.TryGetValue(summary.VacancyReference.Value, out var review))
            {
                summary.NoOfSuccessfulApplications = review.SuccessfulApplications;
                summary.NoOfUnsuccessfulApplications = review.UnsuccessfulApplications;
                summary.NoOfNewApplications = review.NewApplications;
                summary.NoOfAllSharedApplications = review.AllSharedApplications;
                summary.NoOfSharedApplications = review.SharedApplications;
                summary.NoOfEmployerReviewedApplications = review.EmployerReviewedApplications;
            }

            return summary;
        });

        return TypedResults.Ok(vacancySummaries);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Vacancy), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> PostOne(
        [FromServices] IVacancyRepository repository,
        [FromServices] IUserRepository userRepository,
        [FromBody] PostVacancyRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.ToDomain();

        // This lookup should eventually be removed once we've migrated away from Mongo
        // We do this because currently the submitted user id is not the SQL user id, but could match
        // the IdamsUserId, DfEUserId or the actual UserId.
        if (request.SubmittedByUserId is not null)
        {
            var user = await userRepository.FindByUserIdAsync(request.SubmittedByUserId, cancellationToken);
            if (user is not null)
            {
                entity.SubmittedByUserId = user.Id;
            }
        }
        
        // This lookup should eventually be removed once we've migrated away from Mongo
        // We do this because currently the submitted user id is not the SQL user id, but could match
        // the IdamsUserId, DfEUserId or the actual UserId.
        if (request.ReviewRequestedByUserId is not null)
        {
            var user = await userRepository.FindByUserIdAsync(request.ReviewRequestedByUserId, cancellationToken);
            if (user is not null)
            {
                entity.ReviewRequestedByUserId = user.Id;
            }
        }
        
        var vacancyReference = await repository.GetNextVacancyReferenceAsync(cancellationToken);
        entity.VacancyReference = vacancyReference.Value;
        entity.CreatedDate = DateTime.UtcNow;
        
        var result = await repository.UpsertOneAsync(entity, cancellationToken);
        return TypedResults.Created($"/{RouteNames.Vacancies}/{result.Entity.Id}", result.Entity.ToPostResponse());
    }
    
    [HttpPut, Route("{vacancyId:guid}")]
    [ProducesResponseType(typeof(Vacancy), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Vacancy), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]   
    public async Task<IResult> PutOne(
        [FromServices] IVacancyRepository repository,
        [FromServices] IUserRepository userRepository,
        [FromRoute] Guid vacancyId,
        [FromBody] PutVacancyRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.ToDomain(vacancyId);
        
        // This lookup should eventually be removed once we've migrated away from Mongo
        // We do this because currently the submitted user id is not the SQL user id, but could match
        // the IdamsUserId, DfEUserId or the actual UserId.
        if (request.SubmittedByUserId is not null)
        {
            var user = await userRepository.FindByUserIdAsync(request.SubmittedByUserId, cancellationToken);
            if (user is not null)
            {
                entity.SubmittedByUserId = user.Id;
            }
        }
        
        // This lookup should eventually be removed once we've migrated away from Mongo
        // We do this because currently the submitted user id is not the SQL user id, but could match
        // the IdamsUserId, DfEUserId or the actual UserId.
        if (request.ReviewRequestedByUserId is not null)
        {
            var user = await userRepository.FindByUserIdAsync(request.ReviewRequestedByUserId, cancellationToken);
            if (user is not null)
            {
                entity.ReviewRequestedByUserId = user.Id;
            }
        }

        var result = await repository.UpsertOneAsync(entity, cancellationToken);
        
        return result.Created
            ? TypedResults.Created($"/{RouteNames.Vacancies}/{result.Entity.Id}", result.Entity.ToPutResponse())
            : TypedResults.Ok(result.Entity.ToPutResponse());
    }
    
    [HttpPatch, Route("{vacancyId:guid}")]
    [ProducesResponseType(typeof(Vacancy), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> PatchOne(
        [FromServices] IVacancyRepository repository,
        [FromRoute] Guid vacancyId,
        [FromBody] JsonPatchDocument<Vacancy> patchRequest,
        CancellationToken cancellationToken)
    {
        var vacancy = await repository.GetOneAsync(vacancyId, cancellationToken);
        if (vacancy is null)
        {
            return Results.NotFound();
        }
        
        try
        {
            patchRequest.ThrowIfOperationsOn([
                nameof(Vacancy.Id),
                nameof(Vacancy.ApprenticeshipType),
                nameof(Vacancy.OwnerType),
                nameof(Vacancy.VacancyReference),
                nameof(Vacancy.CreatedDate),
                nameof(Vacancy.AccountId),
            ]);
            
            var patchDocument = patchRequest.ToDomain<Vacancy, VacancyEntity>();
            patchDocument.ApplyTo(vacancy);
        }
        catch (JsonPatchException ex)
        {
            return TypedResults.ValidationProblem(ex.ToProblemsDictionary());
        }
    
        await repository.UpsertOneAsync(vacancy, cancellationToken);
        return TypedResults.Ok(vacancy.ToPatchResponse());
    }

    [HttpDelete]
    [Route("{vacancyId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> DeleteOne(
        [FromServices] IVacancyRepository repository,
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        try
        {
            bool deleted = await repository.DeleteOneAsync(vacancyId, cancellationToken);
            return deleted
                ? Results.NoContent()
                : Results.NotFound();
        }
        catch (CannotDeleteVacancyException ex)
        {
            return Results.Problem(ex.ToProblemDetails());
        }
    }
}