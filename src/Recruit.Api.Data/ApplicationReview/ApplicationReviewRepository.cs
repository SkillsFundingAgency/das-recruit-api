using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.ApplicationReview;

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
    Task<List<ApplicationReviewEntity>> GetAllSharedByAccountId(long accountId,
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
    Task<List<ApplicationReviewEntity>> GetAllByAccountId(long accountId, CancellationToken token = default);
    Task<List<ApplicationReviewEntity>> GetAllByUkprn(int ukprn, CancellationToken token = default);
    Task<List<ApplicationReviewEntity>> GetByUkprnAndVacancyReferencesAsync(int ukprn, List<long> vacancyReferences, CancellationToken token = default);
    Task<List<ApplicationReviewEntity>> GetByAccountIdAndVacancyReferencesAsync(long accountId, List<long> vacancyReferences, CancellationToken token = default);
    Task<ApplicationReviewEntity?> GetByApplicationId(Guid applicationId, CancellationToken token = default);
    Task<List<ApplicationReviewEntity>> GetAllByVacancyReference(long vacancyReference, CancellationToken token = default);
    Task<List<ApplicationReviewEntity>> GetNewSharedByAccountId(long accountId, List<long> vacancyReferences,CancellationToken token = default);
    Task<List<ApplicationReviewEntity>> GetAllSharedByAccountId(long accountId, List<long> vacancyReferences, CancellationToken token = default);
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
        return await query.GetPagedAsync(pageNumber, pageSize, sortColumn, isAscending, token);
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
        return await query.GetPagedAsync(pageNumber, pageSize, sortColumn, isAscending, token);
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

        var statusStrings = status is null
            ? [ApplicationReviewStatus.New.ToString()]
            : status.Select(s => s.ToString()).ToList();

        var query = recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview => appReview.AccountId == accountId && statusStrings.Contains(appReview.Status) && appReview.WithdrawnDate == null) 
            .Join(
                recruitDataContext.VacancyReviewEntities.AsNoTracking()
                    .Where(vacancyReview =>
                        vacancyReview.Status == ReviewStatus.Closed &&
                        vacancyReview.ManualOutcome == "Approved" &&
                        ownerTypes.Contains(vacancyReview.OwnerType)),
                appReview => appReview.VacancyReference,
                vacancyReview => vacancyReview.VacancyReference,
                (appReview, _) => appReview
            );

        return await query.GetPagedAsync(pageNumber, pageSize, sortColumn, isAscending, token);
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
            .Where(appReview => appReview.AccountId == accountId && appReview.DateSharedWithEmployer != null)
            .Join(
                recruitDataContext.VacancyReviewEntities.AsNoTracking()
                    .Where(vacancyReview =>
                        vacancyReview.Status == ReviewStatus.Closed &&
                        vacancyReview.ManualOutcome == "Approved"),
                appReview => appReview.VacancyReference,
                vacancyReview => vacancyReview.VacancyReference,
                (appReview, vacancyReview) => appReview
            );

        return await query.GetPagedAsync(pageNumber, pageSize, sortColumn, isAscending, token);
    }

    public async Task<List<ApplicationReviewEntity>> GetAllSharedByAccountId(long accountId,
        CancellationToken token = default)
    {
        var defaultDate = new DateTime(1900, 1, 1, 1, 0, 0, 389, DateTimeKind.Utc);

        var query = recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview =>
                appReview.AccountId == accountId &&
                appReview.DateSharedWithEmployer != null &&
                appReview.DateSharedWithEmployer > defaultDate &&
                appReview.WithdrawnDate == null)
            .Join(
                recruitDataContext.VacancyReviewEntities
                    .AsNoTracking()
                    .Where(vacancyReview =>
                        vacancyReview.Status == ReviewStatus.Closed &&
                        vacancyReview.ManualOutcome == "Approved"),
                appReview => appReview.VacancyReference,
                vacancyReview => vacancyReview.VacancyReference,
                (appReview, vacancyReview) => appReview
            );

        return await query.ToListAsync(token);
    }

    public async Task<List<ApplicationReviewEntity>> GetAllSharedByAccountId(long accountId,
        List<long> vacancyReferences,
        CancellationToken token = default)
    {
        var defaultDate = new DateTime(1900, 1, 1, 1, 0, 0, 389, DateTimeKind.Utc);

        var query = recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview =>
                vacancyReferences.Contains(appReview.VacancyReference) &&
                appReview.AccountId == accountId &&
                appReview.DateSharedWithEmployer != null &&
                appReview.DateSharedWithEmployer > defaultDate &&
                appReview.WithdrawnDate == null)
            .Join(
                recruitDataContext.VacancyReviewEntities
                    .AsNoTracking()
                    .Where(vacancyReview =>
                        vacancyReview.Status == ReviewStatus.Closed &&
                        vacancyReview.ManualOutcome == "Approved"),
                appReview => appReview.VacancyReference,
                vacancyReview => vacancyReview.VacancyReference,
                (appReview, vacancyReview) => appReview
            );

        return await query.ToListAsync(token);
    }

    public async Task<List<ApplicationReviewEntity>> GetNewSharedByAccountId(long accountId,
        List<long> vacancyReferences,
        CancellationToken token = default)
    {
        var query = recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview =>
                vacancyReferences.Contains(appReview.VacancyReference) &&
                appReview.AccountId == accountId &&
                appReview.Status == ApplicationReviewStatus.Shared.ToString() &&
                appReview.WithdrawnDate == null)
            .Join(
                recruitDataContext.VacancyReviewEntities
                    .AsNoTracking()
                    .Where(vacancyReview =>
                        vacancyReview.Status == ReviewStatus.Closed &&
                        vacancyReview.ManualOutcome == "Approved"),
                appReview => appReview.VacancyReference,
                vacancyReview => vacancyReview.VacancyReference,
                (appReview, vacancyReview) => appReview
            );

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
        var statusString = new List<string>();
        if (status == null)
        {
            statusString.Add(ApplicationReviewStatus.New.ToString());
        }
        else
        {
            statusString = status.Select(s => s.ToString()).ToList();
        }

        var query = recruitDataContext.ApplicationReviewEntities
        .AsNoTracking()
            .Where(appReview => appReview.Ukprn == ukprn && statusString.Contains(appReview.Status))
            .Join(
                recruitDataContext.VacancyReviewEntities.AsNoTracking()
                    .Where(vacancyReview =>
                        vacancyReview.Status == ReviewStatus.Closed &&
                        vacancyReview.ManualOutcome == "Approved" &&
                        vacancyReview.OwnerType == OwnerType.Provider),
                appReview => appReview.VacancyReference,
                vacancyReview => vacancyReview.VacancyReference,
                (appReview, _) => appReview
            );

        return await query.GetPagedAsync(pageNumber, pageSize, sortColumn, isAscending, token);
    }

    public async Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        var applicationReview = await recruitDataContext.ApplicationReviewEntities.FirstOrDefaultAsync(fil => fil.Id == entity.Id, token);
        if (applicationReview == null)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await recruitDataContext.ApplicationReviewEntities.AddAsync(entity, token);
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

    public async Task<List<ApplicationReviewEntity>> GetAllByAccountId(long accountId, CancellationToken token = default)
    {
        return await recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview => appReview.AccountId == accountId)
            .Join(
                recruitDataContext.VacancyReviewEntities.AsNoTracking()
                    .Where(vacancyReview =>
                        vacancyReview.Status == ReviewStatus.Closed &&
                        vacancyReview.ManualOutcome == "Approved" &&
                        vacancyReview.OwnerType == OwnerType.Employer),
                appReview => appReview.VacancyReference,
                vacancyReview => vacancyReview.VacancyReference,
                (appReview, _) => appReview
            )
            .ToListAsync(token);
    }

    public async Task<List<ApplicationReviewEntity>> GetAllByUkprn(int ukprn, CancellationToken token = default)
    {
        return await recruitDataContext.ApplicationReviewEntities
        .AsNoTracking()
            .Where(appReview => appReview.Ukprn == ukprn)
            .Join(
                recruitDataContext.VacancyReviewEntities.AsNoTracking()
                    .Where(vacancyReview =>
                        vacancyReview.Status == ReviewStatus.Closed &&
                        vacancyReview.ManualOutcome == "Approved" &&
                        vacancyReview.OwnerType == OwnerType.Provider),
                appReview => appReview.VacancyReference,
                vacancyReview => vacancyReview.VacancyReference,
                (appReview, _) => appReview
            )
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
                recruitDataContext.VacancyReviewEntities.AsNoTracking()
                    .Where(vacancyReview =>
                        vacancyReview.Status == ReviewStatus.Closed &&
                        vacancyReview.ManualOutcome == "Approved" &&
                        vacancyReview.OwnerType == OwnerType.Employer),
                appReview => appReview.VacancyReference,
                vacancyReview => vacancyReview.VacancyReference,
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
                recruitDataContext.VacancyReviewEntities.AsNoTracking()
                    .Where(vacancyReview =>
                        vacancyReview.Status == ReviewStatus.Closed &&
                        vacancyReview.ManualOutcome == "Approved" &&
                        vacancyReview.OwnerType == OwnerType.Provider),
                appReview => appReview.VacancyReference,
                vacancyReview => vacancyReview.VacancyReference,
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
}