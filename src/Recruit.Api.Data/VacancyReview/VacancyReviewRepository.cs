using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.VacancyReview;

public interface IVacancyReviewRepository: IReadRepository<VacancyReviewEntity, Guid>, IWriteRepository<VacancyReviewEntity, Guid>
{
    Task<List<VacancyReviewEntity>> GetManyByVacancyReference(VacancyReference vacancyReference, CancellationToken cancellationToken);
    Task<QaDashboard> GetQaDashboard(CancellationToken cancellationToken);
}

public class VacancyReviewRepository(IRecruitDataContext dataContext): IVacancyReviewRepository
{
    public Task<VacancyReviewEntity?> GetOneAsync(Guid key, CancellationToken cancellationToken)
    {
        return dataContext.VacancyReviewEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == key, cancellationToken);
    }

    public async Task<UpsertResult<VacancyReviewEntity>> UpsertOneAsync(VacancyReviewEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await dataContext.VacancyReviewEntities.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
        if (existingEntity is null)
        {
            await dataContext.VacancyReviewEntities.AddAsync(entity, cancellationToken);
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
        if (entity is null)
        {
            return false;
        }
        
        dataContext.VacancyReviewEntities.Remove(entity);
        await dataContext.SaveChangesAsync(cancellationToken);
        return true;
    }
    
    public Task<List<VacancyReviewEntity>> GetManyByVacancyReference(VacancyReference vacancyReference, CancellationToken cancellationToken)
    {
        return dataContext.VacancyReviewEntities
            .AsNoTracking()
            .Where(x => x.VacancyReference == vacancyReference)
            .OrderBy(x => x.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<QaDashboard> GetQaDashboard(CancellationToken cancellationToken)
    {
        var reviews = await dataContext.VacancyReviewEntities
            .Where(rv => rv.Status == ReviewStatus.PendingReview || rv.Status == ReviewStatus.UnderReview)
            .OrderByDescending(rv => rv.CreatedDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (reviews.Count == 0)
            return new QaDashboard();

        return new QaDashboard 
        {
            TotalVacanciesForReview = reviews.Count,
            TotalVacanciesResubmitted = reviews.Where(r => r.SubmissionCount > 1)
                .Select(r => r.VacancyReference)
                .Distinct()
                .Count(),
            TotalVacanciesBrokenSla = reviews.Count(r => r.SlaDeadLine < DateTime.UtcNow),
            TotalVacanciesSubmittedTwelveTwentyFourHours = reviews.Count(c =>
                c.SubmissionCount == 1 && (DateTime.UtcNow - c.CreatedDate).TotalHours >= 12 && (DateTime.UtcNow - c.CreatedDate).TotalHours < 24)
        };
    }
}