using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Extensions;
using SFA.DAS.Recruit.Api.Data.User;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.User;
using SFA.DAS.Recruit.Api.Models.Responses.User;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route(RouteNames.User)]
public class UserController
{
    private static Dictionary<string, Func<object, Operation<RecruitUser>, Operation<UserEntity>>> PatchFieldMappings { get; } = new() {
        {
            nameof(PutUserRequest.EmployerAccountIds), (key, operation) =>
            {
                return operation.OperationType switch {
                    OperationType.Replace => new Operation<UserEntity>
                    {
                        path = nameof(UserEntity.EmployerAccounts),
                        op = operation.op,
                        value = (operation.value as JArray)!.Select(x => new UserEmployerAccountEntity
                            { UserId = (Guid)key, EmployerAccountId = x.Value<long>()! })
                    },
                    _ => throw new JsonPatchException(new JsonPatchError(null, operation, $"Operation type '{operation.op}' not supported for property '{nameof(UserEntity.EmployerAccounts)}'"))
                };
            }
        },
        {
            nameof(PutUserRequest.NotificationPreferences), (_, operation) =>
            {
                var value = operation.value as JObject;
                return operation.OperationType switch {
                    OperationType.Replace => new Operation<UserEntity>
                    {
                        path = nameof(UserEntity.NotificationPreferences),
                        op = operation.op,
                        value = value?.ToString(Formatting.None) ?? operation.value.ToString()
                    },
                    _ => throw new JsonPatchException(new JsonPatchError(null, operation, $"Operation type '{operation.op}' not supported for property '{nameof(UserEntity.NotificationPreferences)}'"))
                };
            }
        }
    };
    
    [HttpGet, Route("{id:guid}")]
    [ProducesResponseType(typeof(RecruitUser), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOne(
        [FromServices] IUserRepository repository,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetOneAsync(id, cancellationToken);
        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result.ToGetResponse());
    }
    
    [HttpGet, Route("by/employerAccountId/{employerAccountId}")]
    [ProducesResponseType(typeof(List<RecruitUser>), StatusCodes.Status200OK)]
    public async Task<IResult> GetAllByEmployerAccountId(
        [FromServices] IUserRepository repository,
        [FromRoute] long employerAccountId,
        CancellationToken cancellationToken)
    {
        var result = await repository.FindUsersByEmployerAccountIdAsync(employerAccountId, cancellationToken);
        return TypedResults.Ok(result.Select(x => x.ToGetResponse()));
    }
    
    [HttpGet, Route("by/ukprn/{ukprn:long}")]
    [ProducesResponseType(typeof(List<RecruitUser>), StatusCodes.Status200OK)]
    public async Task<IResult> GetAllByUkprn(
        [FromServices] IUserRepository repository,
        [FromRoute] long ukprn,
        CancellationToken cancellationToken)
    {
        var result = await repository.FindUsersByUkprnAsync(ukprn, cancellationToken);
        return TypedResults.Ok(result.Select(x => x.ToGetResponse()));
    }
    
    [HttpGet, Route("by/dfeuserid/{dfeUserId}")]
    [ProducesResponseType(typeof(RecruitUser), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOneByDfeUserId(
        [FromServices] IUserRepository repository,
        [FromRoute] string dfeUserId,
        CancellationToken cancellationToken)
    {
        var result = await repository.FindUsersByDfeUserIdAsync(dfeUserId, cancellationToken);
        return result is null
            ? Results.NotFound()
            : TypedResults.Ok(result.ToGetResponse());
    }
    
    [HttpGet, Route("by/idams/{idams}")]
    [ProducesResponseType(typeof(RecruitUser), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOneByIdams(
        [FromServices] IUserRepository repository,
        [FromRoute] string idams,
        CancellationToken cancellationToken)
    {
        var result = await repository.FindUserByIdamsAsync(idams, cancellationToken);
        return result is null
            ? Results.NotFound()
            : TypedResults.Ok(result.ToGetResponse());
    }
    
    [HttpPut, Route("{id:guid}")]
    [ProducesResponseType(typeof(PutUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PutUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]   
    public async Task<IResult> PutOne(
        [FromServices] IUserRepository repository,
        [FromRoute] Guid id,
        [FromBody] PutUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await repository.UpsertOneAsync(request.ToDomain(id), cancellationToken);

        return result.Created
            ? TypedResults.Created($"/{RouteNames.User}/{result.Entity.Id}", result.Entity.ToPutResponse())
            : TypedResults.Ok(result.Entity.ToPutResponse());
    }
    
    [HttpPatch, Route("{id:guid}")]
    [ProducesResponseType(typeof(RecruitUser), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> PatchOne(
        [FromServices] IUserRepository repository,
        [FromRoute] Guid id,
        [FromBody] JsonPatchDocument<RecruitUser> patchRequest,
        CancellationToken cancellationToken)
    {
        var user = await repository.GetOneAsync(id, cancellationToken);
        if (user is null)
        {
            return Results.NotFound();
        }
        
        try
        {
            patchRequest.ThrowIfOperationsOn([
                nameof(RecruitUser.Id),
                nameof(RecruitUser.CreatedDate),
                nameof(RecruitUser.UserType),
            ]);
            var patchDocument = patchRequest.ToDomain(id, PatchFieldMappings);
            patchDocument.ApplyTo(user);
        }
        catch (JsonPatchException ex)
        {
            return TypedResults.ValidationProblem(ex.ToProblemsDictionary());
        }
    
        await repository.UpsertOneAsync(user, cancellationToken);
        return TypedResults.Ok(user.ToPatchResponse());
    }
}