using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.Repositories;

public interface INotificationsRepository
{
    Task<List<RecruitNotificationEntity>> GetBatchByDateAsync(DateTime when, CancellationToken cancellationToken);
    Task DeleteManyAsync(IEnumerable<long> keys, CancellationToken cancellationToken);
    Task<List<RecruitNotificationEntity>> InsertManyAsync(List<RecruitNotificationEntity> notifications, CancellationToken cancellationToken);
    Task<List<RecruitNotificationEntity>> GetBatchByUserInactiveStatusAsync(CancellationToken cancellationToken);
}

public class NotificationsRepository(IRecruitDataContext dataContext): INotificationsRepository
{
    public async Task<List<RecruitNotificationEntity>> GetBatchByDateAsync(DateTime when, CancellationToken cancellationToken)
    {
        DateTime cutOffDateTime = DateTime.UtcNow.AddYears(-1);

        var userId = await dataContext.RecruitNotifications
            .Where(x =>
                x.SendWhen < when &&
                x.User.LastSignedInDate != null &&
                x.User.LastSignedInDate > cutOffDateTime)
            .Select(x => x.UserId)
            .Distinct()
            .OrderBy(x => x)                
            .FirstOrDefaultAsync(cancellationToken); 

        return await dataContext.RecruitNotifications
            .Include(x => x.User)
            .Where(x =>
                x.SendWhen < when &&
                x.UserId == userId)
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

    public async Task<List<RecruitNotificationEntity>> GetBatchByUserInactiveStatusAsync(CancellationToken cancellationToken)
    {
        DateTime cutOffDateTime = DateTime.UtcNow.AddYears(-1);

        return await dataContext.RecruitNotifications
            .Include(x => x.User)
            .Where(x =>
                x.User.LastSignedInDate == null ||
                x.User.LastSignedInDate < cutOffDateTime)
            .ToListAsync(cancellationToken);
    }
}