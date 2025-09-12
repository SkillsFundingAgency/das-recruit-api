using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Email.ApplicationReview;
using SFA.DAS.Recruit.Api.Core.Exceptions;
using SFA.DAS.Recruit.Api.Core.Extensions;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route(RouteNames.Notifications)]
public class NotificationController : ControllerBase
{
    [HttpGet, Route("batch/by/sendwhen/{sendWhen:datetime}")]
    [ProducesResponseType(typeof(IEnumerable<RecruitNotification>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetBatchByDate(
        [FromRoute] DateTime sendWhen,
        [FromServices] INotificationsRepository repository,
        CancellationToken cancellationToken)
    {
        var results = await repository.GetBatchByDateAsync(sendWhen, cancellationToken);
        return TypedResults.Ok(results.ToResponseDto());
    }
    
    [HttpDelete]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IResult DeleteMany(
        [FromQuery] List<long> ids,
        [FromServices] INotificationsRepository repository,
        CancellationToken cancellationToken)
    {
        if (ids is not { Count: > 0 })
        {
            return TypedResults.BadRequest();
        }
        
        Response.OnCompleted(async () => { await repository.DeleteManyAsync(ids, cancellationToken); });
        return TypedResults.NoContent();
    }


    [HttpPost, Route($"~/{RouteNames.ApplicationReview}/{{id:guid}}/create-notifications")]
    [ProducesResponseType(typeof(IEnumerable<NotificationEmail>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public async Task<IResult> CreateApplicationReviewNotifications(
        [FromServices] IApplicationReviewRepository applicationReviewsRepository,
        [FromServices] IApplicationReviewEmailStrategyFactory factory,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
        )
    {
        var applicationReview = await applicationReviewsRepository.GetById(id, cancellationToken);
        if (applicationReview is null)
        {
            return TypedResults.BadRequest("The specified application review does not exist");
        }

        try
        {
            var strategy = factory.Create(applicationReview);
            var results = await strategy.ExecuteAsync(applicationReview, cancellationToken);
            return TypedResults.Ok(results);
        }
        catch (DataIntegrityException ex)
        {
            return ex.ToResponse();
        }
        catch (MissingEmailStrategyException ex)
        {
            return ex.ToResponse();
        }
    }
}