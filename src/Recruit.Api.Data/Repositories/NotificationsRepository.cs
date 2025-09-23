using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.Repositories;

public interface INotificationsRepository
{
    Task<List<RecruitNotificationEntity>> GetBatchByDateAsync(DateTime when, CancellationToken cancellationToken);
    Task DeleteManyAsync(IEnumerable<long> keys, CancellationToken cancellationToken);
    Task<List<RecruitNotificationEntity>> InsertManyAsync(List<RecruitNotificationEntity> notifications, CancellationToken cancellationToken);
}

public class NotificationsRepository(IRecruitDataContext dataContext): INotificationsRepository
{
    public async Task<List<RecruitNotificationEntity>> GetBatchByDateAsync(DateTime when, CancellationToken cancellationToken)
    {
        const int numberOfUniqueUsers = 1;
        var userIds = await dataContext.RecruitNotifications
            .Where(x => x.SendWhen < when)
            .Select(x => x.UserId)
            .Distinct()
            .Take(numberOfUniqueUsers)
            .ToListAsync(cancellationToken);
        
        return await dataContext.RecruitNotifications
            .Include(x => x.User)
            .Where(x => x.SendWhen < when && userIds.Contains(x.UserId))
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteManyAsync(IEnumerable<long> keys, CancellationToken cancellationToken)
    {
        await dataContext.RecruitNotifications
            .Where(x => keys.Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<List<RecruitNotificationEntity>> InsertManyAsync(List<RecruitNotificationEntity> notifications, CancellationToken cancellationToken)
    {
        dataContext.RecruitNotifications.AddRange(notifications);
        await dataContext.SaveChangesAsync(cancellationToken);
        return notifications;
    }
}