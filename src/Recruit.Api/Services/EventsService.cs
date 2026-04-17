using Esfa.Recruit.Vacancies.Client.Domain.Events;
using NServiceBus;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core.Events;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Validators.Rules;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SFA.DAS.Recruit.Api.Services;

public interface IEventsService
{
    Task PublishVacancyReviewCreatedEventAsync(VacancyReviewEntity entity);
    Task HandleVacancyStatusChange(UpsertResult<VacancyEntity> result);
}

public class EventsService(ILogger<EventsService> logger, IMessageSession messageSession, IEncodingService encodingService): IEventsService
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
    
    public async Task HandleVacancyStatusChange(UpsertResult<VacancyEntity> result)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.StatusChanged is not true)
        {
            return;
        }

        switch (result.Entity.Status)
        {
            case VacancyStatus.Live:
                await messageSession.Publish(new VacancyLiveEvent(result.Entity.Id, result.Entity.VacancyReference!.Value));
                break;
            case VacancyStatus.Closed:
                await messageSession.Publish(new VacancyClosedEvent { VacancyId = result.Entity.Id, VacancyReference = result.Entity.VacancyReference!.Value });
                break;
            case VacancyStatus.Approved:
                await messageSession.Publish(new VacancyApprovedEvent
                {
                    AccountLegalEntityPublicHashedId = encodingService.Encode(result.Entity.AccountLegalEntityId!.Value, EncodingType.AccountLegalEntityId),
                    Ukprn = result.Entity.Ukprn!.Value,
                    VacancyId = result.Entity.Id,
                    VacancyReference = result.Entity.VacancyReference!.Value,
                });
                break;
            case VacancyStatus.Submitted:
                await messageSession.Publish(new VacancySubmittedEvent(result.Entity.Id));
                break;
        }
    }
}