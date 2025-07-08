using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.Vacancy;

public interface IVacancyRepository: IReadRepository<VacancyEntity, Guid>, IWriteRepository<VacancyEntity, Guid>
{
    Task<VacancyReference> GetNextVacancyReferenceAsync(CancellationToken cancellationToken);
}

public class VacancyRepository(IRecruitDataContext dataContext) : IVacancyRepository
{
    public async Task<VacancyEntity?> GetOneAsync(Guid key, CancellationToken cancellationToken)
    {
        return await dataContext
            .VacancyEntities
            .FirstOrDefaultAsync(x => x.Id == key, cancellationToken);
    }

    public async Task<UpsertResult<VacancyEntity>> UpsertOneAsync(VacancyEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await GetOneAsync(entity.Id, cancellationToken);
        if (existingEntity is null)
        {
            await dataContext.VacancyEntities.AddAsync(entity, cancellationToken);
            await dataContext.SaveChangesAsync(cancellationToken);
            return UpsertResult.Create(entity, true);
        }

        dataContext.SetValues(existingEntity, entity);
        await dataContext.SaveChangesAsync(cancellationToken);
        return UpsertResult.Create(entity, false);
    }

    public async Task<bool> DeleteOneAsync(Guid key, CancellationToken cancellationToken)
    {
        var entity = await GetOneAsync(key, cancellationToken);
        switch (entity) {
            case null: return false;
            case { Status: VacancyStatus.Draft or VacancyStatus.Referred or VacancyStatus.Rejected, ClosedDate: null }:
            case { Status: VacancyStatus.Submitted, ClosedDate: null } when entity.ClosingDate < DateTime.UtcNow:
                entity.DeletedDate = DateTime.UtcNow;
                await dataContext.SaveChangesAsync(cancellationToken);
                return true;
            default: 
                throw new CannotDeleteVacancyException();    
        }
    }

    public async Task<VacancyReference> GetNextVacancyReferenceAsync(CancellationToken cancellationToken)
    {
        return await dataContext.GetNextVacancyReferenceAsync(cancellationToken);
    }
}