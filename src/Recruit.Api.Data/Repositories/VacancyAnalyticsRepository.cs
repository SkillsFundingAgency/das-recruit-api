using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.Repositories;

public interface IVacancyAnalyticsRepository : IReadRepository<VacancyAnalyticsEntity, long>,
    IWriteRepository<VacancyAnalyticsEntity, long>;

public class VacancyAnalyticsRepository(IRecruitDataContext dataContext) : IVacancyAnalyticsRepository
{
    public async Task<VacancyAnalyticsEntity?> GetOneAsync(long key, CancellationToken cancellationToken)
    {
        return await dataContext
            .VacancyAnalyticsEntities
            .FirstOrDefaultAsync(x => x.VacancyReference == key, cancellationToken);
    }

    public async Task<UpsertResult<VacancyAnalyticsEntity>> UpsertOneAsync(VacancyAnalyticsEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await GetOneAsync(entity.VacancyReference, cancellationToken);
        if (existingEntity is null)
        {
            await dataContext.VacancyAnalyticsEntities.AddAsync(entity, cancellationToken);
            await dataContext.SaveChangesAsync(cancellationToken);
            return UpsertResult.Create(entity, true);
        }

        dataContext.SetValues(existingEntity, entity);
        await dataContext.SaveChangesAsync(cancellationToken);
        return UpsertResult.Create(entity, false);
    }

    public Task<bool> DeleteOneAsync(long key, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
