using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.Repositories;

public interface IVacancyRepository : IReadRepository<VacancyEntity, Guid>, IWriteRepository<VacancyEntity, Guid>
{
    Task<VacancyReference> GetNextVacancyReferenceAsync(CancellationToken cancellationToken);
    Task<PaginatedList<VacancyEntity>> GetManyByAccountIdAsync<TKey>(long accountId,
        ushort page,
        ushort pageSize,
        Expression<Func<VacancyEntity, TKey>> orderBy,
        SortOrder sortOrder,
        FilteringOptions filteringOptions,
        string searchTerm,
        CancellationToken cancellationToken);
    Task<PaginatedList<VacancyEntity>> GetManyByUkprnIdAsync<TKey>(int ukprn,
        ushort page,
        ushort pageSize,
        Expression<Func<VacancyEntity, TKey>> orderBy,
        SortOrder sortOrder,
        FilteringOptions filteringOptions,
        string searchTerm,
        CancellationToken cancellationToken);
    Task<VacancyEntity?> GetOneByVacancyReferenceAsync(long vacancyReference, CancellationToken cancellationToken);
    Task<List<VacancyEntity>> GetAllByAccountId(long accountId, CancellationToken cancellationToken);
    Task<List<VacancyEntity>> GetAllByUkprn(int ukprn, CancellationToken cancellationToken);
    Task<VacancyEntity?> GetOneClosedVacancyByVacancyReference(VacancyReference vacancyReference, CancellationToken cancellationToken);
    Task<List<VacancyEntity>> GetManyClosedVacanciesByVacancyReferences(List<long> vacancyReference, CancellationToken cancellationToken);
}

public class VacancyRepository(IRecruitDataContext dataContext) : IVacancyRepository
{
    private const int ClosingSoonDays = 5;

    public async Task<PaginatedList<VacancyEntity>> GetManyByAccountIdAsync<TKey>(long accountId,
        ushort page = 1,
        ushort pageSize = 25,
        Expression<Func<VacancyEntity, TKey>>? orderBy = null,
        SortOrder sortOrder = SortOrder.Desc,
        FilteringOptions filteringOptions = FilteringOptions.All,
        string searchTerm = "",
        CancellationToken cancellationToken = default)
    {
        var closingSoonThreshold = DateTime.UtcNow.AddDays(ClosingSoonDays);

        IQueryable<VacancyEntity> query = dataContext.VacancyEntities
            .AsNoTracking()
            .Where(x => x.AccountId == accountId);

        // Apply filters and search
        query = ApplyBasicFiltering(query, filteringOptions);
        query = ApplyClosingSoonFilterByAccountId(query, filteringOptions, closingSoonThreshold, accountId);
        query = ApplySearchTerm(query, searchTerm);

        // Apply owner-type filtering
        query = filteringOptions switch {
            FilteringOptions.Review =>
                query.Where(x => x.OwnerType == OwnerType.Provider || x.OwnerType == OwnerType.Employer),

            FilteringOptions.AllSharedApplications or FilteringOptions.NewSharedApplications =>
                ApplySharedFilteringByAccountId(query, filteringOptions, accountId)
                    .Where(x => x.OwnerType == OwnerType.Provider || x.OwnerType == OwnerType.Employer),

            FilteringOptions.NewApplications or FilteringOptions.AllApplications =>
                ApplySharedFilteringByAccountId(query, filteringOptions, accountId)
                    .Where(x => x.OwnerType == OwnerType.Employer),

            FilteringOptions.All =>
                query.Where(x =>
                    x.OwnerType == OwnerType.Employer ||
                    ((x.OwnerType == OwnerType.Provider || x.OwnerType == OwnerType.Employer)
                     && x.Status == VacancyStatus.Review)),

            _ => query.Where(x => x.OwnerType == OwnerType.Employer)
        };

        // Apply sorting
        if (orderBy is not null)
        {
            query = sortOrder == SortOrder.Desc
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }

        // Paging
        int count = await query.CountAsync(cancellationToken);
        int skip = (Math.Max(page, (ushort)1) - 1) * pageSize;
        int take = Math.Max(pageSize, (ushort)1);

        var items = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return new PaginatedList<VacancyEntity>(items, count, page, pageSize);
    }

    public async Task<PaginatedList<VacancyEntity>> GetManyByUkprnIdAsync<TKey>(int ukprn,
        ushort page = 1,
        ushort pageSize = 25,
        Expression<Func<VacancyEntity, TKey>>? orderBy = null,
        SortOrder sortOrder = SortOrder.Desc,
        FilteringOptions filteringOptions = FilteringOptions.All,
        string searchTerm = "",
        CancellationToken cancellationToken = default)
    {
        var closingSoonThreshold = DateTime.UtcNow.AddDays(ClosingSoonDays);

        IQueryable<VacancyEntity> query = dataContext.VacancyEntities
            .Where(x => x.Ukprn == ukprn && x.OwnerType == OwnerType.Provider);

        // Apply filters
        query = ApplyBasicFiltering(query, filteringOptions);
        query = ApplyClosingSoonFilterByUkprn(query, filteringOptions, closingSoonThreshold, ukprn);

        // Apply search term
        query = ApplySearchTerm(query, searchTerm);
        

        // Apply shared filtering if needed
        if (filteringOptions is FilteringOptions.EmployerReviewedApplications
            or FilteringOptions.AllApplications
            or FilteringOptions.NewApplications)
        {
            query = ApplySharedFilteringByUkprn(query, filteringOptions, ukprn);
        }

        // Apply sorting
        if (orderBy is not null)
        {
            query = sortOrder == SortOrder.Desc
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }

        int count = await query.CountAsync(cancellationToken);
        int skip = (Math.Max(page, (ushort)1) - 1) * pageSize;
        int take = Math.Max(pageSize, (ushort)1);

        var items = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return new PaginatedList<VacancyEntity>(items, count, page, pageSize);
    }

    public async Task<VacancyEntity?> GetOneByVacancyReferenceAsync(long vacancyReference, CancellationToken cancellationToken)
    {
        return await dataContext
            .VacancyEntities
            .FirstOrDefaultAsync(x => x.VacancyReference == vacancyReference, cancellationToken);
    }

    public async Task<List<VacancyEntity>> GetAllByAccountId(long accountId, CancellationToken cancellationToken)
    {
        var employerQuery = dataContext.VacancyEntities
            .AsNoTracking()
            .Where(v => v.AccountId == accountId && v.OwnerType == OwnerType.Employer);

        var providerQuery = dataContext.VacancyEntities
            .AsNoTracking()
            .Where(v => v.AccountId == accountId && v.Status == VacancyStatus.Review && (v.OwnerType == OwnerType.Provider || v.OwnerType == OwnerType.Employer));

        return await employerQuery
            .Union(providerQuery)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<VacancyEntity>> GetAllByUkprn(int ukprn, CancellationToken cancellationToken)
    {
        return await dataContext.VacancyEntities
            .AsNoTracking()
            .Where(vacancy => vacancy.Ukprn == ukprn && vacancy.OwnerType == OwnerType.Provider)
            .ToListAsync(cancellationToken);
    }

    public async Task<VacancyEntity?> GetOneAsync(Guid key, CancellationToken cancellationToken)
    {
        return await dataContext
            .VacancyEntities
            .FirstOrDefaultAsync(x => x.Id == key, cancellationToken);
    }

    public async Task<UpsertResult<VacancyEntity>> UpsertOneAsync(VacancyEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = entity.Id == Guid.Empty ? null : await GetOneAsync(entity.Id, cancellationToken);
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
        switch (entity)
        {
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

    public async Task<VacancyEntity?> GetOneClosedVacancyByVacancyReference(VacancyReference vacancyReference, CancellationToken cancellationToken)
    {
        return await dataContext.VacancyEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.VacancyReference == vacancyReference.Value 
                     && x.Status == VacancyStatus.Closed,
                cancellationToken);
    }

    public async Task<List<VacancyEntity>> GetManyClosedVacanciesByVacancyReferences(List<long> vacancyReferences, CancellationToken cancellationToken)
    {
        return await dataContext.VacancyEntities
            .AsNoTracking()
            .Where(
                x => vacancyReferences.Contains(x.VacancyReference.GetValueOrDefault())
                     && x.Status == VacancyStatus.Closed)
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<VacancyEntity> ApplyBasicFiltering(IQueryable<VacancyEntity> query,
        FilteringOptions filteringOptions)
    {
        return filteringOptions switch {
            // --- Simple status-based filters ---
            FilteringOptions.Draft => query.Where(x => x.Status == VacancyStatus.Draft),
            FilteringOptions.Review => query.Where(x => x.Status == VacancyStatus.Review),
            FilteringOptions.Submitted => query.Where(x => x.Status == VacancyStatus.Submitted),
            FilteringOptions.Live => query.Where(x => x.Status == VacancyStatus.Live),
            FilteringOptions.Closed => query.Where(x => x.Status == VacancyStatus.Closed),
            FilteringOptions.Referred => query.Where(x =>
                                            x.Status == VacancyStatus.Referred ||
                                            x.Status == VacancyStatus.Rejected),
            FilteringOptions.Transferred => query.Where(v => v.TransferInfo != null),

            // --- Application-based filters ---
            FilteringOptions.NewApplications or FilteringOptions.AllApplications =>
                query.Where(x => x.Status == VacancyStatus.Live || x.Status == VacancyStatus.Closed),

            FilteringOptions.NewSharedApplications or FilteringOptions.AllSharedApplications =>
                query.Where(x =>
                    (x.Status == VacancyStatus.Live || x.Status == VacancyStatus.Closed) &&
                    x.OwnerType == OwnerType.Provider),

            FilteringOptions.Dashboard =>
                query.Where(x =>
                    x.Status == VacancyStatus.Live || x.Status == VacancyStatus.Closed),

            _ => query
        };
    }

    private static IQueryable<VacancyEntity> ApplySearchTerm(IQueryable<VacancyEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        searchTerm = searchTerm.Trim().ToLowerInvariant();

        // Try parsing the vacancy reference
        bool isValidVacancyReference = long.TryParse(
            searchTerm.Replace("vac", "", StringComparison.CurrentCultureIgnoreCase), out long vacancyReference);

        query = query.Where(v =>
            (!string.IsNullOrEmpty(v.Title) && v.Title.ToLower().Contains(searchTerm)) ||
            (!string.IsNullOrEmpty(v.LegalEntityName) && v.LegalEntityName.ToLower().Contains(searchTerm)) ||
            (isValidVacancyReference && v.VacancyReference == vacancyReference)
        );

        return query;
    }
    
    private IQueryable<VacancyEntity> ApplySharedFilteringByUkprn(IQueryable<VacancyEntity> query,
        FilteringOptions filteringOptions,
        int ukprn)
    {
        var applicationReviewStatusList = filteringOptions switch {
            FilteringOptions.EmployerReviewedApplications => new[]
            {
                ApplicationReviewStatus.EmployerInterviewing,
                ApplicationReviewStatus.EmployerUnsuccessful
            },
            FilteringOptions.AllApplications => new[]
            {
                ApplicationReviewStatus.New,
                ApplicationReviewStatus.Unsuccessful,
                ApplicationReviewStatus.Successful
            },
            FilteringOptions.NewApplications => new[]
            {
                ApplicationReviewStatus.New
            },
            _ => Array.Empty<ApplicationReviewStatus>()
        };

        if (applicationReviewStatusList.Length == 0)
        {
            return query.Where(_ => false);
        }

        var filteredVacancyRefs = dataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(appReview =>
                appReview.Ukprn == ukprn &&
                applicationReviewStatusList.Contains(appReview.Status) &&
                appReview.WithdrawnDate == null)
            .Select(appReview => appReview.VacancyReference)
            .Distinct();

        return query
            .Where(v => v.VacancyReference.HasValue)
            .Where(v => filteredVacancyRefs.Contains(v.VacancyReference.GetValueOrDefault()));
    }

    private IQueryable<VacancyEntity> ApplySharedFilteringByAccountId(IQueryable<VacancyEntity> query,
        FilteringOptions filteringOptions,
        long accountId)
    {
        IQueryable<ApplicationReviewEntity> appQuery = filteringOptions switch {
            FilteringOptions.AllSharedApplications =>
                dataContext.ApplicationReviewEntities
                    .AsNoTracking()
                    .Where(appReview =>
                        appReview.AccountId == accountId &&
                        appReview.DateSharedWithEmployer != null &&
                        appReview.WithdrawnDate == null),

            FilteringOptions.NewSharedApplications =>
                dataContext.ApplicationReviewEntities
                    .AsNoTracking()
                    .Where(appReview =>
                        appReview.AccountId == accountId &&
                        appReview.Status == ApplicationReviewStatus.Shared &&
                        appReview.WithdrawnDate == null),

            FilteringOptions.AllApplications =>
                dataContext.ApplicationReviewEntities
                    .AsNoTracking()
                    .Where(appReview =>
                        appReview.AccountId == accountId &&
                        (appReview.Status == ApplicationReviewStatus.New
                         || appReview.Status == ApplicationReviewStatus.Unsuccessful
                         || appReview.Status == ApplicationReviewStatus.Successful) &&
                        appReview.WithdrawnDate == null),

            FilteringOptions.NewApplications =>
                dataContext.ApplicationReviewEntities
                    .AsNoTracking()
                    .Where(appReview =>
                        appReview.AccountId == accountId &&
                        appReview.Status == ApplicationReviewStatus.New &&
                        appReview.WithdrawnDate == null),

            _ => Enumerable.Empty<ApplicationReviewEntity>().AsQueryable()
        };

        if (appQuery.Expression.Type == typeof(EnumerableQuery<ApplicationReviewEntity>))
        {
            // No results possible – return empty query using same provider
            return query.Where(_ => false);
        }

        var filteredVacancyRefs = appQuery
            .Select(appReview => appReview.VacancyReference)
            .Distinct();

        return query
            .Where(v => v.VacancyReference.HasValue)
            .Where(v => filteredVacancyRefs.Contains(v.VacancyReference.GetValueOrDefault()));
    }

    private IQueryable<VacancyEntity> ApplyClosingSoonFilterByAccountId(
        IQueryable<VacancyEntity> query,
        FilteringOptions filteringOptions,
        DateTime closingSoonThreshold,
        long accountId)
    {
        switch (filteringOptions)
        {
            case FilteringOptions.ClosingSoon:
                return query.Where(x =>
                    x.Status == VacancyStatus.Live &&
                    x.ClosingDate <= closingSoonThreshold);
            case FilteringOptions.ClosingSoonWithNoApplications:
                {
                    var activeApplications = dataContext.ApplicationReviewEntities
                        .AsNoTracking()
                        .Where(a => a.WithdrawnDate == null && a.AccountId == accountId)
                        .Select(a => a.VacancyReference);

                    return query.Where(v =>
                        v.Status == VacancyStatus.Live &&
                        v.ClosingDate <= closingSoonThreshold &&
                        v.ApplicationMethod == ApplicationMethod.ThroughFindAnApprenticeship &&
                        v.VacancyReference.HasValue &&
                        !activeApplications.Contains(v.VacancyReference.Value));
                }
            default:
                return query;
        }
    }

    private IQueryable<VacancyEntity> ApplyClosingSoonFilterByUkprn(
        IQueryable<VacancyEntity> query,
        FilteringOptions filteringOptions,
        DateTime closingSoonThreshold,
        int ukprn)
    {
        switch (filteringOptions)
        {
            case FilteringOptions.ClosingSoon:
                return query.Where(x =>
                    x.Status == VacancyStatus.Live &&
                    x.ClosingDate <= closingSoonThreshold);
            case FilteringOptions.ClosingSoonWithNoApplications:
                {
                    var activeApplications = dataContext.ApplicationReviewEntities
                        .AsNoTracking()
                        .Where(a => a.WithdrawnDate == null && a.Ukprn == ukprn)
                        .Select(a => a.VacancyReference);

                    return query.Where(v =>
                        v.Status == VacancyStatus.Live &&
                        v.ClosingDate <= closingSoonThreshold &&
                        v.ApplicationMethod == ApplicationMethod.ThroughFindAnApprenticeship &&
                        v.VacancyReference.HasValue &&
                        !activeApplications.Contains(v.VacancyReference.Value));
                }
            default:
                return query;
        }
    }
}