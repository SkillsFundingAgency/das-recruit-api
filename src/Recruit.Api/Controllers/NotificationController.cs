using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.ApplicationReview;
using SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;
using SFA.DAS.Recruit.Api.Core.Exceptions;
using SFA.DAS.Recruit.Api.Core.Extensions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Responses.Notifications;

namespace SFA.DAS.Recruit.Api.Controllers;

[ApiController, Route(RouteNames.Notifications)]
public class NotificationController : ControllerBase
{
    [HttpGet, Route("batch/by/date")]
    [ProducesResponseType(typeof(GetBatchByDateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetBatchByDate(
        [FromQuery, Required] DateTime? dateTime,
        [FromServices] INotificationsRepository repository,
        [FromServices] IEmailFactory emailFactory,
        CancellationToken cancellationToken)
    {
        var recruitNotifications = await repository.GetBatchByDateAsync(dateTime!.Value, cancellationToken);
        var results = emailFactory.CreateFrom(recruitNotifications);
        return TypedResults.Ok(new GetBatchByDateResponse(results));
    }
    
    [HttpDelete]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> DeleteMany(
        [FromQuery] List<long> ids,
        [FromServices] INotificationsRepository repository,
        CancellationToken cancellationToken)
    {
        if (ids is not { Count: > 0 })
        {
            return TypedResults.BadRequest();
        }
        
        await repository.DeleteManyAsync(ids, cancellationToken);
        return TypedResults.NoContent();
    }

    [HttpPost, Route($"~/{RouteNames.ApplicationReview}/{{id:guid}}/create-notifications")]
    [ProducesResponseType(typeof(IEnumerable<NotificationEmail>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public async Task<IResult> CreateApplicationReviewNotifications(
        [FromServices] IApplicationReviewRepository applicationReviewsRepository,
        [FromServices] INotificationsRepository notificationsRepository,
        [FromServices] IApplicationReviewNotificationStrategy strategy,
        [FromServices] IEmailFactory emailfactory,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
        )
    {
        var applicationReview = await applicationReviewsRepository.GetById(id, cancellationToken);
        if (applicationReview is null)
        {
            return TypedResults.NotFound("The specified application review does not exist");
        }

        try
        {
            var notificationFactory = strategy.Create(applicationReview);
            var recruitNotifications = await notificationFactory.CreateAsync(applicationReview, cancellationToken);
            if (recruitNotifications.Delayed is { Count: > 0 })
            {
                await notificationsRepository.InsertManyAsync(recruitNotifications.Delayed, cancellationToken);
            }
            var results = emailfactory.CreateFrom(recruitNotifications.Immediate);
            return TypedResults.Ok(results);
        }
        catch (DataIntegrityException ex)
        {
            return ex.ToResponse();
        }
        catch (EntityStateNotSupportedException ex)
        {
            return ex.ToResponse();
        }
    }
    
    [HttpPost, Route($"~/{RouteNames.Vacancies}/{{id:guid}}/create-notifications")]
    [ProducesResponseType(typeof(IEnumerable<NotificationEmail>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public async Task<IResult> CreateVacancySubmittedNotifications(
        [FromServices] IVacancyRepository vacancyRepository,
        [FromServices] INotificationsRepository notificationsRepository,
        [FromServices] IVacancyNotificationStrategy strategy,
        [FromServices] IEmailFactory emailfactory,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var vacancy = await vacancyRepository.GetOneAsync(id, cancellationToken);
        if (vacancy is null)
        {
            return TypedResults.NotFound("The specified vacancy does not exist");
        }

        try
        {
            var notificationFactory = strategy.Create(vacancy);
            var recruitNotifications = await notificationFactory.CreateAsync(vacancy, cancellationToken);
            if (recruitNotifications.Delayed is { Count: > 0 })
            {
                await notificationsRepository.InsertManyAsync(recruitNotifications.Delayed, cancellationToken);
            }
            var results = emailfactory.CreateFrom(recruitNotifications.Immediate);
            return TypedResults.Ok(results);
        }
        catch (EntityStateNotSupportedException ex)
        {
            return ex.ToResponse();
        }
    }
}