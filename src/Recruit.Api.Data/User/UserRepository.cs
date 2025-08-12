using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.User;

public interface IUserRepository : IReadRepository<UserEntity, Guid>, IWriteRepository<UserEntity, Guid>
{
    Task<UserEntity?> FindByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task<List<UserEntity>> FindUsersByEmployerAccountIdAsync(string employerAccountId, CancellationToken cancellationToken);
    Task<List<UserEntity>> FindUsersByUkprnAsync(long ukprn, CancellationToken cancellationToken);
}

public class UserRepository(IRecruitDataContext dataContext) : IUserRepository
{
    public async Task<UserEntity?> GetOneAsync(Guid key, CancellationToken cancellationToken)
    {
        return await dataContext.UserEntities
            .Include(x => x.EmployerAccounts)
            .FirstOrDefaultAsync(x => x.Id == key, cancellationToken);
    }

    public async Task<UpsertResult<UserEntity>> UpsertOneAsync(UserEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await GetOneAsync(entity.Id, cancellationToken);
        if (existingEntity is null)
        {
            dataContext.UserEntities.Add(entity);
            await dataContext.SaveChangesAsync(cancellationToken);
            return UpsertResult.Create(entity, true);
        }
        entity.UpdatedDate = DateTime.UtcNow;
        dataContext.SetValues(existingEntity, entity);

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
        return await dataContext.UserEntities
            .Include(x => x.EmployerAccounts)
            .Where(x => x.IdamsUserId == userId || x.DfEUserId == userId || x.Id.ToString() == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<UserEntity>> FindUsersByEmployerAccountIdAsync(string employerAccountId, CancellationToken cancellationToken)
    {
        var results = await dataContext
            .UserEmployerAccountEntities
            .Include(x => x.User)
            .Where(x => x.EmployerAccountId == employerAccountId)
            .ToListAsync(cancellationToken);
        
        return results.Select(x => x.User).ToList();
    }

    public async Task<List<UserEntity>> FindUsersByUkprnAsync(long ukprn, CancellationToken cancellationToken)
    {
        return await dataContext
            .UserEntities
            .Where(x => x.Ukprn == ukprn)
            .ToListAsync(cancellationToken);
    }
}