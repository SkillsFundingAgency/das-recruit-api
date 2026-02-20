using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Extensions;
using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Models;
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
using SFA.DAS.Recruit.Api.Models.Responses.Vacancy;
using SFA.DAS.Recruit.Api.Validators;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

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

    [HttpGet, Route("{vacancyReference:long}/live")]
    [ProducesResponseType(typeof(Vacancy), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOneLiveVacancyByVacancyReference(
        [FromServices] IVacancyRepository repository,
        [FromRoute] long vacancyReference,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetOneLiveVacancyByVacancyReferenceAsync(vacancyReference, cancellationToken);

        if (result == null)
        {
            return Results.NotFound();
        }
        
        var vacancy = result.ToGetResponse();
        vacancy.AddWageData();
        return TypedResults.Ok(vacancy);
    }

    [HttpGet, Route("live")]
    [ProducesResponseType(typeof(PagedResponse<Vacancy>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetManyLiveVacancies(
        [FromServices] IVacancyRepository repository,
        PagingParams pagingParams,
        [FromQuery] DateTime? closingDate,
        CancellationToken cancellationToken)
    {
        ushort page = pagingParams.Page ?? 1;
        ushort pageSize = pagingParams.PageSize ?? 25;

        var vacancies = await repository.GetManyLiveVacancies(page, pageSize, closingDate, cancellationToken);

        return TypedResults.Ok(vacancies.ToPagedResponse(x =>
        {
            var response = x.ToGetResponse();
            response.AddWageData();
            return response;
        }));
    }

    [HttpGet, Route("{vacancyReference:long}/closed")]
    [ProducesResponseType(typeof(Vacancy), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOneClosedVacancyByVacancyReference(
        [FromServices] IVacancyRepository repository,
        [FromRoute] long vacancyReference,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await repository.GetOneClosedVacancyByVacancyReference(vacancyReference, cancellationToken);
            if (result == null)
            {
                return Results.NotFound();
            }
        
            var vacancy = result.ToGetResponse();
            vacancy.AddWageData();
            return TypedResults.Ok(vacancy);
        }
        catch (InvalidVacancyReferenceException)
        {
            return Results.NotFound();
        }
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
            .Select(v =>
            {
                var response = v.ToGetResponse();
                response.AddWageData();
                return response;
            })
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


    [HttpGet]
    [Route($"{RouteElements.TotalPositionsAvailable}")]
    [ProducesResponseType(typeof(TotalPositionsAvailableResponse), StatusCodes.Status200OK)]
    public async Task<IResult> GetTotalPositionsAvailable(
        [FromServices] IVacancyRepository repository,
        CancellationToken cancellationToken = default)
    {
        int response = await repository.GetLiveVacanciesCountAsync(cancellationToken);
        return TypedResults.Ok(new TotalPositionsAvailableResponse(response));
    }

    [HttpPost]
    [ProducesResponseType(typeof(Vacancy), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> PostOne(
        [FromServices] IVacancyRepository repository,
        [FromServices] IUserRepository userRepository,
        [FromServices] IValidator<VacancyRequest> validator,
        [FromBody] PostVacancyRequest request,
        [FromQuery] VacancyRuleSet? ruleSet,
        CancellationToken cancellationToken,
        [FromQuery] bool validateOnly = false)
    {
        if (ruleSet != null)
        {
            var context = new ValidationContext<VacancyRequest>(request);
            context.RootContextData.Add(ValidationConstants.ValidationsRulesKey, ruleSet);
            var validationResult = await validator.ValidateAsync(context, cancellationToken);
            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }    
        }
        
        var entity = request.ToDomain();
        
        if (validateOnly)
        {
            entity.VacancyReference = 1000000001;
            entity.CreatedDate = DateTime.UtcNow;
            entity.Status = VacancyStatus.Submitted;
            entity.Id = Guid.NewGuid();
            return TypedResults.Created($"/{RouteNames.Vacancies}/{entity.Id}", entity.ToPutResponse());
        }
        

        // This lookup should eventually be removed once we've migrated away from Mongo
        // We do this because currently the submitted user id is not the SQL user id, but could match
        // the IdamsUserId, DfEUserId or the actual UserId.
        if (request.SubmittedByUserId is not null)
        {
            var userId = await userRepository.FindIdByUserIdAsync(request.SubmittedByUserId, cancellationToken);
            if (userId is not null)
            {
                entity.SubmittedByUserId = userId;
            }
        }
        
        // This lookup should eventually be removed once we've migrated away from Mongo
        // We do this because currently the submitted user id is not the SQL user id, but could match
        // the IdamsUserId, DfEUserId or the actual UserId.
        if (request.ReviewRequestedByUserId is not null)
        {
            var userId = await userRepository.FindIdByUserIdAsync(request.ReviewRequestedByUserId, cancellationToken);
            if (userId is not null)
            {
                entity.ReviewRequestedByUserId = userId;
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
        [FromServices] IValidator<VacancyRequest> validator,
        [FromRoute] Guid vacancyId,
        [FromBody] PutVacancyRequest request,
        [FromQuery] VacancyRuleSet? ruleSet,
        CancellationToken cancellationToken,
        [FromQuery] bool validateOnly = false)
    {
        if (ruleSet != null)
        {
            var context = new ValidationContext<VacancyRequest>(request);
            context.RootContextData.Add(ValidationConstants.ValidationsRulesKey, ruleSet);
            var validationResult = await validator.ValidateAsync(context, cancellationToken);
            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }    
        }
        
        if (validateOnly)
        {
            var id = Guid.NewGuid();
            return TypedResults.Created($"/{RouteNames.Vacancies}/{id}",
                new VacancyEntity { Id = id, Status = VacancyStatus.Submitted, VacancyReference = 1000000001 }
                    .ToPutResponse());
        }
        
        var entity = request.ToDomain(vacancyId);
        
        // This lookup should eventually be removed once we've migrated away from Mongo
        // We do this because currently the submitted user id is not the SQL user id, but could match
        // the IdamsUserId, DfEUserId or the actual UserId.
        if (request.SubmittedByUserId is not null)
        {
            var userId = await userRepository.FindIdByUserIdAsync(request.SubmittedByUserId, cancellationToken);
            if (userId is not null)
            {
                entity.SubmittedByUserId = userId;
            }
        }
        
        // This lookup should eventually be removed once we've migrated away from Mongo
        // We do this because currently the submitted user id is not the SQL user id, but could match
        // the IdamsUserId, DfEUserId or the actual UserId.
        if (request.ReviewRequestedByUserId is not null)
        {
            var userId = await userRepository.FindIdByUserIdAsync(request.ReviewRequestedByUserId, cancellationToken);
            if (userId is not null)
            {
                entity.ReviewRequestedByUserId = userId;
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
        var vacancyEntity = await repository.GetOneAsync(vacancyId, cancellationToken);
        if (vacancyEntity is null)
        {
            return Results.NotFound();
        }
        
        try
        {
            patchRequest.ThrowIfOperationsOn([
                nameof(Vacancy.Id),
                nameof(Vacancy.ApprenticeshipType),
                nameof(Vacancy.VacancyReference),
                nameof(Vacancy.CreatedDate),
                nameof(Vacancy.AccountId),
            ]);

            var vacancy = VacancyMapper.FromEntity(vacancyEntity);
            patchRequest.ApplyTo(vacancy);
            vacancyEntity = VacancyMapper.ToEntity(vacancy);
        }
        catch (JsonPatchException ex)
        {
            return TypedResults.ValidationProblem(ex.ToProblemsDictionary());
        }
    
        await repository.UpsertOneAsync(vacancyEntity, cancellationToken);
        return TypedResults.Ok(vacancyEntity.ToPatchResponse());
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
            var deleted = await repository.DeleteOneAsync(vacancyId, cancellationToken);
            return deleted
                ? Results.NoContent()
                : Results.NotFound();
        }
        catch (CannotDeleteVacancyException ex)
        {
            return Results.Problem(ex.ToProblemDetails());
        }
    }

    [HttpGet]
    [Route("count/user/{userId:guid}")]
    [ProducesResponseType(typeof(DataResponse<int>), StatusCodes.Status200OK)]
    public async Task<IResult> CountByUserId(
        [FromRoute] Guid userId,
        [FromServices] IUserRepository userRepository,
        [FromServices] IVacancyRepository vacancyRepository,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByUserIdAsync($"{userId}", cancellationToken);
        if (user is null)
        {
            return TypedResults.Ok(new DataResponse<int>(0)); 
        }

        var count = await vacancyRepository.CountVacanciesByUserIdAsync(user.Id, cancellationToken);
        return TypedResults.Ok(new DataResponse<int>(count));
    }
}