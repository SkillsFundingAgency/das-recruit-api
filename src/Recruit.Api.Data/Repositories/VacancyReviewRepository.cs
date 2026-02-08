using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.Repositories;

public interface IVacancyReviewRepository: IReadRepository<VacancyReviewEntity, Guid>, IWriteRepository<VacancyReviewEntity, Guid>
{
    Task<List<VacancyReviewEntity>> GetManyByVacancyReference(VacancyReference vacancyReference, CancellationToken cancellationToken);
    Task<List<VacancyReviewEntity>> GetManyByVacancyReferenceAndStatus(VacancyReference vacancyReference, IReadOnlyCollection<ReviewStatus> statuses, IReadOnlyCollection<string>? manualOutcome, bool includeNoStatus, CancellationToken cancellationToken);
    Task<QaDashboard> GetQaDashboard(CancellationToken cancellationToken);
    Task<List<VacancyReviewEntity>> GetManyByStatusAndExpiredAssignationDateTime(
        IReadOnlyCollection<ReviewStatus> statuses,
        DateTime? expiredAssignationDateTime,
        CancellationToken cancellationToken);
    Task<List<VacancyReviewEntity>> GetManyByAccountLegalEntityId(long accountLegalEntityId, CancellationToken cancellationToken);
    Task<List<VacancyReviewEntity>> GetManyByReviewedByUserEmailAndAssignationExpiry(string reviewedByUserEmail, DateTime? assignationExpiry, ReviewStatus? reviewStatus, CancellationToken cancellationToken);
    Task<int> GetCountBySubmittedUserEmail(string submittedByUserEmail, bool? approvedFirstTime, DateTime? assignationExpiry, CancellationToken cancellationToken);
    Task<int> GetCountByAccountLegalEntityId(
        long accountLegalEntityId,
        IReadOnlyCollection<ReviewStatus>? statuses,
        IReadOnlyCollection<string>? manualOutcome,
        EmployerNameOption? employerNameOption,
        CancellationToken cancellationToken);
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
        
        existingEntity.SubmittedByUserEmail = entity.SubmittedByUserEmail;

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

    public Task<List<VacancyReviewEntity>> GetManyByVacancyReferenceAndStatus(
        VacancyReference vacancyReference,
        IReadOnlyCollection<ReviewStatus> statuses,
        IReadOnlyCollection<string>? manualOutcome,
        bool includeNoStatus,
        CancellationToken cancellationToken)
    {
        var query = dataContext.VacancyReviewEntities
            .AsNoTracking()
            .Where(x => x.VacancyReference == vacancyReference)
            .AsQueryable();

        if (statuses is { Count: > 0 })
        {
            query = query.Where(x => statuses.Contains(x.Status));    
        }

        if (manualOutcome is { Count: > 0 })
        {
            if (includeNoStatus)
            {
                query = query.Where(x => x.ManualOutcome == null || manualOutcome.Contains(x.ManualOutcome));
            }
            else
            {
                query = query.Where(x => manualOutcome.Contains(x.ManualOutcome));    
            }
            
        }

        return query
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

    public Task<List<VacancyReviewEntity>> GetManyByStatusAndExpiredAssignationDateTime(
        IReadOnlyCollection<ReviewStatus> statuses,
        DateTime? expiredAssignationDateTime,
        CancellationToken cancellationToken)
    {
        var query = dataContext.VacancyReviewEntities
            .AsNoTracking()
            .AsQueryable();

        if (statuses is { Count: > 0 })
        {
            query = query.Where(x => statuses.Contains(x.Status));
        }

        if (expiredAssignationDateTime is not null)
        {
            query = query.Where(x => x.ReviewedDate > expiredAssignationDateTime.Value);
        }

        return query.ToListAsync(cancellationToken);
    }

    public Task<List<VacancyReviewEntity>> GetManyByAccountLegalEntityId(long accountLegalEntityId, CancellationToken cancellationToken)
    {
        return dataContext.VacancyReviewEntities
            .AsNoTracking()
            .Where(x => x.AccountLegalEntityId == accountLegalEntityId)
            .OrderBy(x => x.CreatedDate)
            .ToListAsync(cancellationToken);
    }


    public Task<int> GetCountByAccountLegalEntityId(
        long accountLegalEntityId,
        IReadOnlyCollection<ReviewStatus>? statuses,
        IReadOnlyCollection<string>? manualOutcome,
        EmployerNameOption? employerNameOption,
        CancellationToken cancellationToken)
    {
        var query = dataContext.VacancyReviewEntities
            .AsNoTracking()
            .Where(x => x.AccountLegalEntityId == accountLegalEntityId)
            .AsQueryable();

        if (statuses is { Count: > 0 })
        {
            query = query.Where(x => statuses.Contains(x.Status));
        }

        if (manualOutcome is { Count: > 0 })
        {
            query = query.Where(x => manualOutcome.Contains(x.ManualOutcome!));
        }

        if (employerNameOption is not null)
        {
            query = from r in query
                join v in dataContext.VacancyEntities.AsNoTracking()
                    on r.VacancyReference equals v.VacancyReference
                where v.EmployerNameOption == employerNameOption.Value
                select r;
        }

        return query.CountAsync(cancellationToken);
    }

    public Task<List<VacancyReviewEntity>> GetManyByReviewedByUserEmailAndAssignationExpiry(string reviewedByUserEmail, DateTime? assignationExpiry, ReviewStatus? reviewStatus, CancellationToken cancellationToken)
    {
        var query = dataContext.VacancyReviewEntities
            .AsNoTracking()
            .Where(x => x.ReviewedByUserEmail == reviewedByUserEmail)
            .AsQueryable();

        if (assignationExpiry is not null)
        {
            query = query.Where(x => x.ReviewedDate > assignationExpiry.Value);
        }

        if (reviewStatus is not null)
        {
            query = query.Where(x => x.Status == reviewStatus);
        }

        return query
            .OrderBy(x => x.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public Task<int> GetCountBySubmittedUserEmail(string submittedByUserEmail, bool? approvedFirstTime, DateTime? assignationExpiry, CancellationToken cancellationToken)
    {
        var query = dataContext.VacancyReviewEntities
            .AsNoTracking()
            .Where(x => x.SubmittedByUserEmail == submittedByUserEmail)
            .AsQueryable();

        if (assignationExpiry is not null)
        {
            query = query.Where(x => x.ReviewedDate <= assignationExpiry.Value);
        }

        if (approvedFirstTime is true)
        {
            query = query.Where(x => x.SubmissionCount == 1 && x.Status == ReviewStatus.Closed && x.ManualOutcome == "Approved");
        }
        else if (approvedFirstTime is false)
        {
            query = query.Where(x => !(x.SubmissionCount == 1 && x.Status == ReviewStatus.Closed && x.ManualOutcome == "Approved"));
        }

        return query.CountAsync(cancellationToken);
    }
}