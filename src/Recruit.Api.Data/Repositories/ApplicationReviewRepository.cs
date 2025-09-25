using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.Repositories;

public interface IApplicationReviewRepository
{
    Task<ApplicationReviewEntity?> GetById(Guid id, CancellationToken token = default);
    Task<PaginatedList<ApplicationReviewEntity>> GetAllByAccountId(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default);
    Task<PaginatedList<ApplicationReviewEntity>> GetAllByUkprn(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default);
    Task<PaginatedList<ApplicationReviewEntity>> GetPagedByAccountAndStatusAsync(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        List<ApplicationReviewStatus>? status = null,
        CancellationToken token = default);
    Task<int> GetSharedCountByAccountId(long accountId,
        CancellationToken token = default);
    Task<int> GetAllSharedCountByAccountId(long accountId,
        CancellationToken token = default);
    Task<PaginatedList<ApplicationReviewEntity>> GetAllSharedByAccountId(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default);
    Task<PaginatedList<ApplicationReviewEntity>> GetPagedByUkprnAndStatusAsync(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        List<ApplicationReviewStatus>? status = null,
        CancellationToken token = default);
    Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default);
    Task<ApplicationReviewEntity?> Update(ApplicationReviewEntity entity, CancellationToken token = default);
    Task<List<ApplicationReviewsDashboardCountModel>> GetAllByAccountId(long accountId, CancellationToken token = default);
    Task<List<ApplicationReviewsDashboardCountModel>> GetAllByUkprn(int ukprn, CancellationToken token = default);
    Task<List<ApplicationReviewEntity>> GetByUkprnAndVacancyReferencesAsync(int ukprn, List<long> vacancyReferences, CancellationToken token = default);
    Task<List<ApplicationReviewEntity>> GetByAccountIdAndVacancyReferencesAsync(long accountId, List<long> vacancyReferences, CancellationToken token = default);
    Task<ApplicationReviewEntity?> GetByApplicationId(Guid applicationId, CancellationToken token = default);
    Task<List<ApplicationReviewEntity>> GetAllByVacancyReference(long vacancyReference, CancellationToken token = default);
    Task<List<ApplicationReviewEntity>> GetNewSharedByAccountId(long accountId, List<long> vacancyReferences,CancellationToken token = default);
    Task<List<ApplicationReviewEntity>> GetAllSharedByAccountId(long accountId,List<long> vacancyReferences, CancellationToken token = default);
}

internal class ApplicationReviewRepository(IRecruitDataContext recruitDataContext) : IApplicationReviewRepository
{
    public async Task<ApplicationReviewEntity?> GetById(Guid id, CancellationToken token = default)
    {
        return await recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(fil => fil.Id == id, token);
    }
    public async Task<ApplicationReviewEntity?> GetByApplicationId(Guid applicationId, CancellationToken token = default)
    {
        return await recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(fil => fil.ApplicationId == applicationId, token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetAllByAccountId(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default)
    {
        var query = recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(fil => fil.AccountId == accountId);
        var groupedCountQuery = await query.GroupBy(c => c.VacancyReference).CountAsync(cancellationToken: token);
        
        return await query.GetPagedAsync(pageNumber, pageSize, sortColumn, isAscending, groupedCountQuery, token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetAllByUkprn(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default)
    {
        var query = recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(fil => fil.Ukprn == ukprn);
        
        var groupedCountQuery = await query.GroupBy(c => c.VacancyReference).CountAsync(cancellationToken: token);
        
        return await query.GetPagedAsync(pageNumber, pageSize, sortColumn, isAscending, groupedCountQuery, token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetPagedByAccountAndStatusAsync(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        List<ApplicationReviewStatus>? status = null,
        CancellationToken token = default)
    {
        var ownerTypes = status is not null && status.Contains(ApplicationReviewStatus.Shared)
            ? new List<OwnerType> { OwnerType.Employer, OwnerType.Provider }
            : new List<OwnerType> { OwnerType.Employer };

        var statuses = status ?? [ApplicationReviewStatus.New];
        var query = recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview => appReview.AccountId == accountId && statuses.Contains(appReview.Status) && appReview.WithdrawnDate == null) 
            .Join(
                recruitDataContext.VacancyEntities.AsNoTracking()
                    .Where(vacancy => ownerTypes.Contains((OwnerType)vacancy.OwnerType!)),
                appReview => appReview.VacancyReference,
                vacancy => vacancy.VacancyReference,
                (appReview, _) => appReview
            );
        
        var groupedCountQuery = await query.GroupBy(c => c.VacancyReference).CountAsync(cancellationToken: token);

        return await query.GetPagedAsync(pageNumber, pageSize, sortColumn, isAscending, groupedCountQuery, token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetAllSharedByAccountId(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        CancellationToken token = default)
    {
        var query = recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview => appReview.AccountId == accountId && appReview.DateSharedWithEmployer != null && appReview.WithdrawnDate == null)
            .Join(
                recruitDataContext.VacancyEntities.AsNoTracking(),
                appReview => appReview.VacancyReference,
                vacancy => vacancy.VacancyReference,
                (appReview, vacancy) => appReview
            );
        
        var groupedCountQuery = await query.GroupBy(c => c.VacancyReference).CountAsync(cancellationToken: token);

        return await query.GetPagedAsync(pageNumber, pageSize, sortColumn, isAscending, groupedCountQuery, token);
    }
    public async Task<int> GetSharedCountByAccountId(long accountId,
        CancellationToken token = default)
    {
        var query = recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview =>
                appReview.AccountId == accountId &&
                appReview.DateSharedWithEmployer != null &&
                appReview.Status == ApplicationReviewStatus.Shared &&
                appReview.WithdrawnDate == null);

        return await query.CountAsync(token);
    }

    public async Task<int> GetAllSharedCountByAccountId(long accountId, CancellationToken token = default)
    {
        var query = recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview =>
                appReview.AccountId == accountId &&
                appReview.DateSharedWithEmployer != null &&
                appReview.WithdrawnDate == null);

        return await query.CountAsync(token);
    }

    public async Task<List<ApplicationReviewEntity>> GetAllSharedByAccountId(long accountId,List<long> vacancyReferences,
        CancellationToken token = default)
    {
        var query = recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview =>
                vacancyReferences.Contains(appReview.VacancyReference) &&
                appReview.AccountId == accountId &&
                appReview.DateSharedWithEmployer != null &&
                appReview.WithdrawnDate == null);

        return await query.ToListAsync(token);
    }
    public async Task<List<ApplicationReviewEntity>> GetNewSharedByAccountId(long accountId,List<long> vacancyReferences,
        CancellationToken token = default)
    {
        var query = recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview =>
                vacancyReferences.Contains(appReview.VacancyReference) &&
                appReview.AccountId == accountId &&
                appReview.Status == ApplicationReviewStatus.Shared &&
                appReview.WithdrawnDate == null);

        return await query.ToListAsync(token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetPagedByUkprnAndStatusAsync(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = nameof(ApplicationReviewEntity.CreatedDate),
        bool isAscending = false,
        List<ApplicationReviewStatus>? status = null,
        CancellationToken token = default)
    {
        var statuses = status ?? [ApplicationReviewStatus.New];

        var query = recruitDataContext.ApplicationReviewEntities
        .AsNoTracking()
            .Where(appReview =>
                appReview.Ukprn == ukprn &&
                statuses.Contains(appReview.Status) &&
                appReview.WithdrawnDate == null)
            .Join(
                recruitDataContext.VacancyEntities.AsNoTracking()
                    .Where(vacancy => vacancy.OwnerType == OwnerType.Provider),
                appReview => appReview.VacancyReference,
                vacancy => vacancy.VacancyReference,
                (appReview, _) => appReview);

        int groupedCountQuery = await query
            .Select(c => c.VacancyReference)
            .Distinct()
            .CountAsync(cancellationToken: token);

        return await query.GetPagedAsync(pageNumber, pageSize, sortColumn, isAscending, groupedCountQuery, token);
    }

    public async Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        var applicationReview = await recruitDataContext.ApplicationReviewEntities.FirstOrDefaultAsync(fil => fil.Id == entity.Id, token);
        if (applicationReview == null)
        {
            entity.CreatedDate = DateTime.UtcNow;
            recruitDataContext.ApplicationReviewEntities.Add(entity);
            await recruitDataContext.SaveChangesAsync(token);
            return UpsertResult.Create(entity, true);
        }

        recruitDataContext.SetValues(applicationReview, entity);
        await recruitDataContext.SaveChangesAsync(token);
        return UpsertResult.Create(entity, false);
    }

    public async Task<ApplicationReviewEntity?> Update(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        var applicationReview = await recruitDataContext.ApplicationReviewEntities.FirstOrDefaultAsync(fil => fil.Id == entity.Id, token);
        if (applicationReview is null)
        {
            return await UpdateByApplicationId(entity, token);
        }
        
        recruitDataContext.SetValues(applicationReview, entity);
        await recruitDataContext.SaveChangesAsync(token);
        return entity;
    }

    private async Task<ApplicationReviewEntity?> UpdateByApplicationId(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        var applicationReview = await recruitDataContext.ApplicationReviewEntities.FirstOrDefaultAsync(fil => fil.ApplicationId == entity.ApplicationId, token);
        if (applicationReview is null)
        {
            return null;
        }
        entity.Id = applicationReview.Id;
        
        recruitDataContext.SetValues(applicationReview, entity);
        await recruitDataContext.SaveChangesAsync(token);
        return entity;
    }

    public async Task<List<ApplicationReviewsDashboardCountModel>> GetAllByAccountId(long accountId, CancellationToken token = default)
    {
        return await recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview => appReview.AccountId == accountId && appReview.WithdrawnDate == null)
            .Join(
                recruitDataContext.VacancyEntities.AsNoTracking()
                    .Where(vacancy => vacancy.OwnerType == OwnerType.Employer),
                appReview => appReview.VacancyReference,
                vacancy => vacancy.VacancyReference,
                (appReview, _) => appReview
            ).GroupBy(c => c.Status).Select(g => 
                new ApplicationReviewsDashboardCountModel
                {
                    Status = Enum.Parse<ApplicationReviewStatus>(g.Key.ToString(), true),
                    Count = g.Count()
                })
            .ToListAsync(token);
    }

    public async Task<List<ApplicationReviewsDashboardCountModel>> GetAllByUkprn(int ukprn, CancellationToken token = default)
    {
        return await recruitDataContext.ApplicationReviewEntities
        .AsNoTracking()
            .Where(appReview => appReview.Ukprn == ukprn && appReview.WithdrawnDate == null)
            .Join(
                recruitDataContext.VacancyEntities.AsNoTracking()
                    .Where(vacancy => vacancy.OwnerType == OwnerType.Provider),
                appReview => appReview.VacancyReference,
                vacancy => vacancy.VacancyReference,
                (appReview, _) => appReview
            ).GroupBy(c => c.Status).Select(g => 
            new ApplicationReviewsDashboardCountModel
            {
                Status = Enum.Parse<ApplicationReviewStatus>(g.Key.ToString(), true),
                Count = g.Count()
            })
            .ToListAsync(token);
    }

    public async Task<List<ApplicationReviewEntity>> GetByAccountIdAndVacancyReferencesAsync(long accountId,
        List<long> vacancyReferences,
        CancellationToken token = default)
    {
        if (vacancyReferences.Count == 0)
            return [];

        return await recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview => appReview.AccountId == accountId && vacancyReferences.Contains(appReview.VacancyReference))
            .Join(
                recruitDataContext.VacancyEntities.AsNoTracking()
                    .Where(vacancy => vacancy.OwnerType == OwnerType.Employer),
                appReview => appReview.VacancyReference,
                vacancy => vacancy.VacancyReference,
                (appReview, _) => appReview
            )
            .ToListAsync(token);
    }

    public async Task<List<ApplicationReviewEntity>> GetByUkprnAndVacancyReferencesAsync(int ukprn,
        List<long> vacancyReferences,
        CancellationToken token = default)
    {
        if (vacancyReferences.Count == 0)
            return [];

        return await recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview => appReview.Ukprn == ukprn && vacancyReferences.Contains(appReview.VacancyReference))
            .Join(
                recruitDataContext.VacancyEntities.AsNoTracking()
                    .Where(vacancy => vacancy.OwnerType == OwnerType.Provider),
                appReview => appReview.VacancyReference,
                vacancy => vacancy.VacancyReference,
                (appReview, _) => appReview
            )
            .ToListAsync(token);
    }

    public async Task<List<ApplicationReviewEntity>> GetAllByVacancyReference(long vacancyReference, CancellationToken token = default)
    {
        return await recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(fil => fil.VacancyReference == vacancyReference)
            .ToListAsync(token);
    }
}