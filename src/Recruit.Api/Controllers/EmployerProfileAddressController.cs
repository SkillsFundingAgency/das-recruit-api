using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Extensions;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.EmployerProfileAddress;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route("api/[controller]s/{accountLegalEntityId:long}/")]
public class EmployerProfileAddressController: ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployerProfileAddress>), StatusCodes.Status200OK)]
    public async Task<IResult> GetMany(
        [FromServices] IEmployerProfileAddressRepository repository,
        [FromRoute] long accountLegalEntityId,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetManyAsync(accountLegalEntityId, cancellationToken);

        return TypedResults.Ok(result.ToGetResponse());
    }
    
    [HttpGet, Route("{id:int}")]
    [ProducesResponseType(typeof(EmployerProfileAddress), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOne(
        [FromServices] IEmployerProfileAddressRepository repository,
        [FromRoute] long accountLegalEntityId,
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetOneAsync(new EmployerProfileAddressKey(accountLegalEntityId, id), cancellationToken);

        return result is null
            ? Results.NotFound()
            : TypedResults.Ok(result.ToGetResponse());
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(EmployerProfileAddress), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]   
    public async Task<IResult> PostOne(
        [FromServices] IEmployerProfileAddressRepository repository,
        [FromRoute] long accountLegalEntityId,
        [FromBody] PostEmployerProfileAddressRequest request,
        CancellationToken cancellationToken)
    {
        var result = await repository.UpsertOneAsync(request.ToDomain(accountLegalEntityId), cancellationToken);

        return TypedResults.Created($"/api/employerprofiles/{result.Entity.AccountLegalEntityId}", result.Entity.ToPostResponse());
    }
    
    [HttpPatch, Route("{id:int}")]
    [ProducesResponseType(typeof(EmployerProfileAddress), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> PatchOne(
        [FromServices] IEmployerProfileAddressRepository repository,
        [FromRoute] long accountLegalEntityId,
        [FromRoute] int id,
        [FromBody] JsonPatchDocument patchRequest,
        CancellationToken cancellationToken)
    {
        var address = await repository.GetOneAsync(new EmployerProfileAddressKey(accountLegalEntityId, id), cancellationToken);
        if (address is null)
        {
            return Results.NotFound();
        }

        var patchDocument = patchRequest.ToDomain<EmployerProfileAddressEntity>();
        try
        {
            patchDocument.ThrowIfOperationsOn([nameof(EmployerProfileEntity.AccountLegalEntityId)]);
            patchDocument.ApplyTo(address);
        }
        catch (JsonPatchException ex)
        {
            return TypedResults.ValidationProblem(ex.ToProblemsDictionary());
        }

        await repository.UpsertOneAsync(address, cancellationToken);
        return TypedResults.Ok(address.ToPatchResponse());
    }

    [HttpDelete, Route("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteOne(
        [FromServices] IEmployerProfileAddressRepository repository,
        [FromRoute] long accountLegalEntityId,
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        bool deleted = await repository.DeleteOneAsync(new EmployerProfileAddressKey(accountLegalEntityId, id), cancellationToken);

        return deleted
            ? Results.NoContent()
            : Results.NotFound();
    }
}