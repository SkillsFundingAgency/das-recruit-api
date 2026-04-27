using Esfa.Recruit.Vacancies.Client.Domain.Events;
using NServiceBus;
using SFA.DAS.Recruit.Api.Core.Events;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Validators.Rules;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SFA.DAS.Recruit.Api.Services;

public interface IEventsService
{
    Task PublishVacancyReviewCreatedEventAsync(VacancyReviewEntity entity);
    Task PublishVacancyClosedEvent(VacancyEntity entity);
}

public class EventsService(ILogger<EventsService> logger, IMessageSession messageSession): IEventsService
{
    public async Task PublishVacancyReviewCreatedEventAsync(VacancyReviewEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var snapshot = JsonSerializer.Deserialize<VacancySnapshot>(entity.VacancySnapshot, JsonConfig.Options);
        logger.LogInformation("Publishing VacancyReviewCreatedEvent, vacancyReviewId='{VacancyReviewId}, vacancyId='{VacancyId}' ", entity.Id, snapshot!.Id);

        var isResubmission = entity.SubmissionCount > 1;
        var hasPassedAutoQaChecks = string.Equals(entity.AutomatedQaOutcome, nameof(RuleSetDecision.Approve), StringComparison.InvariantCultureIgnoreCase);
        await messageSession.Publish(new VacancyReviewCreatedEvent(snapshot.Id, entity.Id, isResubmission, hasPassedAutoQaChecks));
    }
    
    public async Task PublishVacancyClosedEvent(VacancyEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await messageSession.Publish(new VacancyClosedEvent { VacancyId = entity.Id, VacancyReference = entity.VacancyReference!.Value });
    }
}