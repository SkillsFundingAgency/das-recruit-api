using Esfa.Recruit.Vacancies.Client.Domain.Events;
using NServiceBus;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core.Events;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Validators.Rules;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SFA.DAS.Recruit.Api.Services;

public interface IEventsService
{
    Task HandleVacancyReviewStatusChange(UpsertResult<VacancyReviewEntity> result);
    Task HandleVacancyStatusChange(UpsertResult<VacancyEntity> result);
}

public class EventsService(ILogger<EventsService> logger, IMessageSession messageSession, IEncodingService encodingService): IEventsService
{
    public async Task HandleVacancyReviewStatusChange(UpsertResult<VacancyReviewEntity> result)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.StatusChanged is not true)
        {
            return;
        }

        var snapshot = JsonSerializer.Deserialize<VacancySnapshot>(result.Entity.VacancySnapshot, JsonConfig.Options);
        switch (result.Entity.Status)
        {
            case ReviewStatus.New:
                logger.LogInformation("Publishing VacancyReviewCreatedEvent, vacancyReviewId='{VacancyReviewId}, vacancyId='{VacancyId}' ", result.Entity.Id, snapshot!.Id);
                var isResubmission = result.Entity.SubmissionCount > 1;
                var hasPassedAutoQaChecks = string.Equals(result.Entity.AutomatedQaOutcome, nameof(RuleSetDecision.Approve), StringComparison.InvariantCultureIgnoreCase);
                await messageSession.Publish(new VacancyReviewCreatedEvent(snapshot.Id, result.Entity.Id, isResubmission, hasPassedAutoQaChecks));
                break;
            case ReviewStatus.Closed:
                if (result.Entity.ManualOutcome == nameof(ManualQaOutcome.Approved))
                {
                    logger.LogInformation("Publishing VacancyReviewApprovedEvent, vacancyReviewId='{VacancyReviewId}, vacancyId='{VacancyId}' ", result.Entity.Id, snapshot!.Id);
                    await messageSession.Publish(new VacancyReviewApprovedEvent(result.Entity.Id, snapshot.Id));
                }
                break;
        }
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
                    AccountLegalEntityPublicHashedId = encodingService.Encode(result.Entity.AccountLegalEntityId!.Value, EncodingType.PublicAccountLegalEntityId),
                    Ukprn = result.Entity.Ukprn!.Value,
                    VacancyId = result.Entity.Id,
                    VacancyReference = result.Entity.VacancyReference!.Value,
                });
                break;
        }
    }
}