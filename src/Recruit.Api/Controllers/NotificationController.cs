using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Data.Repositories;
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
}