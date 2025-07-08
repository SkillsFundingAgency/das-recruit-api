using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Extensions;
using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Vacancy;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;

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
    
    [HttpPost]
    [ProducesResponseType(typeof(Vacancy), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> PostOne(
        [FromServices] IVacancyRepository repository,
        [FromBody] PostVacancyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await repository.UpsertOneAsync(request.ToDomain(), cancellationToken);
        return TypedResults.Created($"/{RouteNames.Vacancies}/{result.Entity.VacancyReference}", result.Entity.ToPostResponse());
    }
    
    [HttpPut, Route("{vacancyId:guid}")]
    [ProducesResponseType(typeof(Vacancy), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Vacancy), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]   
    public async Task<IResult> PutOne(
        [FromServices] IVacancyRepository repository,
        [FromRoute] Guid vacancyId,
        [FromBody] PutVacancyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await repository.UpsertOneAsync(request.ToDomain(vacancyId), cancellationToken);
    
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