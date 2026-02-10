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
        
        entity.SubmittedByUserEmail = existingEntity.SubmittedByUserEmail;

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
    
    public async Task<List<VacancyReviewEntity>> GetManyByVacancyReference(VacancyReference vacancyReference, CancellationToken cancellationToken)
    {
        return await dataContext.VacancyReviewEntities
            .AsNoTracking()
            .Where(x => x.VacancyReference == vacancyReference.Value)
            .OrderBy(x => x.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<QaDashboard> GetQaDashboard(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var dashboard = await dataContext.VacancyReviewEntities
            .Where(rv => rv.Status == ReviewStatus.PendingReview || rv.Status == ReviewStatus.UnderReview)
            .AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new QaDashboard {

                // Total vacancies currently awaiting review
                TotalVacanciesForReview = g.Count(),

                // Vacancies that have been resubmitted for review (i.e. more than one submission)
                TotalVacanciesResubmitted = g
                    .Where(r => r.SubmissionCount > 1)
                    .Select(r => r.VacancyReference)
                    .Distinct()
                    .Count(),

                // Vacancies that have breached the SLA (i.e. not reviewed within 24 hours of submission)
                TotalVacanciesBrokenSla = g.Count(r =>
                    r.CreatedDate <= now.AddHours(-24)),

                // Vacancies submitted in the last 24 hours, split into two 12 hour periods
                TotalVacanciesSubmittedLastTwelveHours = g.Count(r =>
                    r.CreatedDate > now.AddHours(-12)),

                // Vacancies submitted between 12 and 24 hours ago
                TotalVacanciesSubmittedTwelveTwentyFourHours = g.Count(r =>
                    r.CreatedDate <= now.AddHours(-12) &&
                    r.CreatedDate > now.AddHours(-24))
            })
            .FirstOrDefaultAsync(cancellationToken);

        return dashboard ?? new QaDashboard();
    }
}