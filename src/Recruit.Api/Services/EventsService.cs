using NServiceBus;
using SFA.DAS.Recruit.Api.Core.Events;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SFA.DAS.Recruit.Api.Services;

public interface IEventsService
{
    Task PublishVacancyReviewCreatedEventAsync(VacancyReviewEntity entity);
}

public class EventsService(IMessageSession messageSession): IEventsService
{
    private class VacancySnapshotProps
    {
        public Guid Id { get; init; } // only field we want
    }
    
    public async Task PublishVacancyReviewCreatedEventAsync(VacancyReviewEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var snapshot = JsonSerializer.Deserialize<VacancySnapshotProps>(entity.VacancySnapshot, JsonConfig.Options);
        await messageSession.Publish(new VacancyReviewCreatedEvent(snapshot!.Id, entity.Id));
    }
}