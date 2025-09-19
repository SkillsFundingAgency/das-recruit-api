using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.Repositories;

public record struct EmployerProfileAddressKey(long AccountLegalEntityId, int Id);

public interface IEmployerProfileAddressRepository :
    IReadRepository<EmployerProfileAddressEntity, EmployerProfileAddressKey>,
    IWriteRepository<EmployerProfileAddressEntity, EmployerProfileAddressKey>
{
    Task<List<EmployerProfileAddressEntity>> GetManyAsync(long accountLegalEntityId, CancellationToken cancellationToken);
}

internal class EmployerProfileAddressRepository(IRecruitDataContext dataContext): IEmployerProfileAddressRepository
{
    public Task<EmployerProfileAddressEntity?> GetOneAsync(EmployerProfileAddressKey key, CancellationToken cancellationToken)
    {
        return dataContext.EmployerProfileAddressEntities
            .FirstOrDefaultAsync(x => x.AccountLegalEntityId == key.AccountLegalEntityId && x.Id == key.Id, cancellationToken);
    }

    public async Task<UpsertResult<EmployerProfileAddressEntity>> UpsertOneAsync(EmployerProfileAddressEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await GetOneAsync(new EmployerProfileAddressKey(entity.AccountLegalEntityId, entity.Id), cancellationToken);
        if (existingEntity is null)
        {
            await dataContext.EmployerProfileAddressEntities.AddAsync(entity, cancellationToken);
            await dataContext.SaveChangesAsync(cancellationToken);
            return UpsertResult.Create(entity, true);
        }

        dataContext.SetValues(existingEntity, entity);
        await dataContext.SaveChangesAsync(cancellationToken);
        return UpsertResult.Create(entity, false);
    }

    public async Task<bool> DeleteOneAsync(EmployerProfileAddressKey key, CancellationToken cancellationToken)
    {
        var entity = await GetOneAsync(key, cancellationToken);
        if (entity is null)
        {
            return false;
        }
        
        dataContext.EmployerProfileAddressEntities.Remove(entity);
        await dataContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<List<EmployerProfileAddressEntity>> GetManyAsync(long accountLegalEntityId, CancellationToken cancellationToken)
    {
        return dataContext.EmployerProfileAddressEntities
            .Where(x => x.AccountLegalEntityId == accountLegalEntityId)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }
}