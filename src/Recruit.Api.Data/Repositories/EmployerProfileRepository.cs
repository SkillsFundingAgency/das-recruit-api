using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.Repositories;

public interface IEmployerProfileRepository : IReadRepository<EmployerProfileEntity, long>, IWriteRepository<EmployerProfileEntity, long>
{
    Task<List<EmployerProfileEntity>> GetManyForAccountAsync(long accountId, CancellationToken cancellationToken);
}

internal class EmployerProfileRepository(IRecruitDataContext dataContext): IEmployerProfileRepository
{
    public Task<EmployerProfileEntity?> GetOneAsync(long accountLegalEntityId, CancellationToken cancellationToken)
    {
        return dataContext.EmployerProfileEntities
            .Include(x => x.Addresses)
            .FirstOrDefaultAsync(x => x.AccountLegalEntityId == accountLegalEntityId, cancellationToken);
    }

    public async Task<UpsertResult<EmployerProfileEntity>> UpsertOneAsync(EmployerProfileEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await GetOneAsync(entity.AccountLegalEntityId, cancellationToken);
        if (existingEntity is null)
        {
            await dataContext.EmployerProfileEntities.AddAsync(entity, cancellationToken);
            await dataContext.SaveChangesAsync(cancellationToken);
            return UpsertResult.Create(entity, true);
        }

        dataContext.SetValues(existingEntity, entity);
        await dataContext.SaveChangesAsync(cancellationToken);
        return UpsertResult.Create(entity, false);
    }

    public async Task<bool> DeleteOneAsync(long accountLegalEntityId, CancellationToken cancellationToken)
    {
        var entity = await GetOneAsync(accountLegalEntityId, cancellationToken);
        if (entity is null)
        {
            return false;
        }
        
        dataContext.EmployerProfileEntities.Remove(entity);
        await dataContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<List<EmployerProfileEntity>> GetManyForAccountAsync(long accountId, CancellationToken cancellationToken)
    {
        return dataContext.EmployerProfileEntities
            .Include(x => x.Addresses)
            .Where(x => x.AccountId == accountId)
            .OrderBy(x => x.AccountLegalEntityId)
            .ToListAsync(cancellationToken);
    }
}