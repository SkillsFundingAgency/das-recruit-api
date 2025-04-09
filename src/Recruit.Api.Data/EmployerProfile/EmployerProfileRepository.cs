using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.EmployerProfile;

public interface IEmployerProfileRepository : IReadRepository<EmployerProfileEntity, long>, IWriteRepository<EmployerProfileEntity, long>
{ }

public class EmployerProfileRepository(IRecruitDataContext dataContext): IEmployerProfileRepository
{
    public Task<EmployerProfileEntity?> GetOneAsync(long accountLegalEntityId, CancellationToken cancellationToken)
    {
        return dataContext.EmployerProfileEntities
            .Include(x => x.Addresses)
            .FirstOrDefaultAsync(x => x.AccountLegalEntityId == accountLegalEntityId, cancellationToken);
    }

    public async Task<UpsertResult<EmployerProfileEntity>> UpsertAsync(EmployerProfileEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await GetOneAsync(entity.AccountLegalEntityId, cancellationToken);
        if (existingEntity is null)
        {
            await dataContext.EmployerProfileEntities.AddAsync(entity, cancellationToken);
            await dataContext.SaveChangesAsync(cancellationToken);
            return UpsertResult.Create(entity, true);
        }

        dataContext.Entry(existingEntity).CurrentValues.SetValues(entity);
        await dataContext.SaveChangesAsync(cancellationToken);
        return UpsertResult.Create(entity, false);
    }

    public async Task<bool> DeleteAsync(long accountLegalEntityId, CancellationToken cancellationToken)
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
}