using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.Repositories;

public interface IUserRepository : IReadRepository<UserEntity, Guid>, IWriteRepository<UserEntity, Guid>
{
    Task<UserEntity?> FindByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task<List<UserEntity>> FindUsersByEmployerAccountIdAsync(long employerAccountId, CancellationToken cancellationToken);
    Task<List<UserEntity>> FindUsersByUkprnAsync(long ukprn, CancellationToken cancellationToken);
    Task<UserEntity?> FindUserByIdamsAsync(string idams, CancellationToken cancellationToken);
    Task<UserEntity?> FindUserByDfeUserIdAsync(string dfeUserId, CancellationToken cancellationToken);
}

public class UserRepository(IRecruitDataContext dataContext) : IUserRepository
{
    public async Task<UserEntity?> GetOneAsync(Guid key, CancellationToken cancellationToken)
    {
        var user = await dataContext.UserEntities
            .Include(x => x.EmployerAccounts)
            .FirstOrDefaultAsync(x => x.Id == key, cancellationToken);
        
        NotificationPreferenceDefaults.Update(user);
        return user;
    }

    public async Task<UpsertResult<UserEntity>> UpsertOneAsync(UserEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await GetOneAsync(entity.Id, cancellationToken);
        if (existingEntity is null)
        {
            await dataContext.UserEntities.AddAsync(entity, cancellationToken);
            await dataContext.SaveChangesAsync(cancellationToken);
            return UpsertResult.Create(entity, true);
        }
        var prefs = existingEntity.NotificationPreferences;
        
        entity.UpdatedDate = DateTime.UtcNow;
        dataContext.SetValues(existingEntity, entity);

        // TODO: temporary fix whilst users are being updated with data from mongo, this
        // should be removed once the switch to SQL is complete
        if (existingEntity.NotificationPreferences?.EventPreferences?.Count == 0)
        {
            existingEntity.NotificationPreferences = prefs;
        }

        // remove the deleted employerAccountIds
        var newEntityIds = entity.EmployerAccounts.Select(x => x.EmployerAccountId).ToList();
        existingEntity.EmployerAccounts.RemoveAll(x => !newEntityIds.Contains(x.EmployerAccountId));

        // add new employerAccountIds
        var oldEntityIds = existingEntity.EmployerAccounts.Select(x => x.EmployerAccountId).ToList();
        var newEmployerAccounts = entity.EmployerAccounts.Where(x => !oldEntityIds.Contains(x.EmployerAccountId)).ToList();
        existingEntity.EmployerAccounts.AddRange(newEmployerAccounts);

        await dataContext.SaveChangesAsync(cancellationToken);
        return UpsertResult.Create(entity, false);
    }

    public Task<bool> DeleteOneAsync(Guid key, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<UserEntity?> FindByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await dataContext.UserEntities
            .Include(x => x.EmployerAccounts)
            .Where(x => x.IdamsUserId == userId || x.DfEUserId == userId || x.Id.ToString() == userId)
            .FirstOrDefaultAsync(cancellationToken);

        NotificationPreferenceDefaults.Update(user);
        return user;
    }

    public async Task<List<UserEntity>> FindUsersByEmployerAccountIdAsync(long employerAccountId, CancellationToken cancellationToken)
    {
        var results = await dataContext
            .UserEmployerAccountEntities
            .Include(x => x.User)
            .Where(x => x.EmployerAccountId == employerAccountId)
            .ToListAsync(cancellationToken);
        
        var users = results.Select(x => x.User).ToList();
        NotificationPreferenceDefaults.Update(users);
        return users;
    }

    public async Task<List<UserEntity>> FindUsersByUkprnAsync(long ukprn, CancellationToken cancellationToken)
    {
        var users = await dataContext
            .UserEntities
            .Where(x => x.Ukprn == ukprn)
            .ToListAsync(cancellationToken);

        NotificationPreferenceDefaults.Update(users);
        return users;
    }

    public async Task<UserEntity?> FindUserByIdamsAsync(string idams, CancellationToken cancellationToken)
    {
        var user = await dataContext.UserEntities.SingleOrDefaultAsync(x => x.IdamsUserId == idams, cancellationToken);
        NotificationPreferenceDefaults.Update(user);
        return user;
    }

    public async Task<UserEntity?> FindUserByDfeUserIdAsync(string dfeUserId, CancellationToken cancellationToken)
    {
        var user = await dataContext.UserEntities.SingleOrDefaultAsync(x => x.DfEUserId == dfeUserId, cancellationToken);
        NotificationPreferenceDefaults.Update(user);
        return user;
    }
}