using System.Text.Json;
using Esfa.Recruit.Vacancies.Client.Domain.Events;
using NServiceBus;
using SFA.DAS.Recruit.Api.Core.Events;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SFA.DAS.Recruit.Api.Services;

public interface IEventsService
{
    Task PublishVacancyReviewCreatedEventAsync(VacancyReviewEntity entity);
    Task PublishVacancyClosedEvent(VacancyEntity entity);
}

public class EventsService(ILogger<EventsService> logger, IMessageSession messageSession): IEventsService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    
    private class VacancySnapshotProps
    {
        public Guid Id { get; init; } // only field we want
    }
    
    public async Task PublishVacancyReviewCreatedEventAsync(VacancyReviewEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var snapshot = JsonSerializer.Deserialize<VacancySnapshotProps>(entity.VacancySnapshot, JsonOptions);
        
        if (entity.VacancySnapshot is not { Length: > 0 })
        {
            logger.LogError("VacancyReviewCreatedEvent publishing for '{VacancyReviewId}' will fail, the snapshot is empty and no VacancyId will be available", entity.Id);
        }
        
        logger.LogInformation("Publishing VacancyReviewCreatedEvent, vacancyReviewId='{VacancyReviewId}, vacancyId='{VacancyId}' ", entity.Id, snapshot!.Id);
        await messageSession.Publish(new VacancyReviewCreatedEvent(snapshot.Id, entity.Id));
    }
    
    public async Task PublishVacancyClosedEvent(VacancyEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await messageSession.Publish(new VacancyClosedEvent { VacancyId = entity.Id, VacancyReference = entity.VacancyReference!.Value });
    }
}