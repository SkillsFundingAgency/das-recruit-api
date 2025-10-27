using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.Repositories;

public interface IVacancyRepository : IReadRepository<VacancyEntity, Guid>, IWriteRepository<VacancyEntity, Guid>
{
    Task<VacancyReference> GetNextVacancyReferenceAsync(CancellationToken cancellationToken);
    Task<PaginatedList<VacancySummaryEntity>> GetManyByAccountIdAsync<TKey>(long accountId,
        ushort page,
        ushort pageSize,
        Expression<Func<VacancyEntity, TKey>> orderBy,
        SortOrder sortOrder,
        FilteringOptions filteringOptions,
        string searchTerm,
        CancellationToken cancellationToken);
    Task<PaginatedList<VacancySummaryEntity>> GetManyByUkprnIdAsync<TKey>(int ukprn,
        ushort page,
        ushort pageSize,
        Expression<Func<VacancyEntity, TKey>> orderBy,
        SortOrder sortOrder,
        FilteringOptions filteringOptions,
        string searchTerm,
        CancellationToken cancellationToken);
    Task<VacancyEntity?> GetOneByVacancyReferenceAsync(long vacancyReference, CancellationToken cancellationToken);
    Task<List<VacancyTransferSummaryEntity>> GetAllTransferInfoByAccountId(long accountId, CancellationToken cancellationToken, bool withTransferInfo = false);
    Task<List<VacancyTransferSummaryEntity>> GetAllTransferInfoByUkprn(int ukprn, CancellationToken cancellationToken, bool withTransferInfo = false);
    Task<VacancyEntity?> GetOneClosedVacancyByVacancyReference(VacancyReference vacancyReference, CancellationToken cancellationToken);
    Task<List<VacancyEntity>> GetManyClosedVacanciesByVacancyReferences(List<long> vacancyReference, CancellationToken cancellationToken);
    Task<List<VacancyDashboardCountModel>> GetEmployerDashboard(long accountId, CancellationToken cancellationToken);
    Task<List<(int, bool)>> GetEmployerVacanciesClosingSoonWithApplications(long accountId, CancellationToken cancellationToken);
    Task<List<VacancyDashboardCountModel>> GetProviderDashboard(int ukprn, CancellationToken cancellationToken);
    Task<List<(int, bool)>> GetProviderVacanciesClosingSoonWithApplications(int ukprn, CancellationToken cancellationToken);

    Task<List<VacancyEntity>> GetAllClosedEmployerVacanciesByClosureReason(long accountId, ClosureReason closureReason, DateTime lastDismissedDate, CancellationToken cancellationToken);
    Task<List<VacancyEntity>> GetAllClosedProviderrVacanciesByClosureReason(int ukprn, ClosureReason closureReason, DateTime lastDismissedDate, CancellationToken cancellationToken);
}

public class VacancyRepository(IRecruitDataContext dataContext) : IVacancyRepository
{
    private const int ClosingSoonDays = 5;
    
    public async Task<PaginatedList<VacancySummaryEntity>> GetManyByAccountIdAsync<TKey>(long accountId,
        ushort page = 1,
        ushort pageSize = 25,
        Expression<Func<VacancyEntity, TKey>>? orderBy = null,
        SortOrder sortOrder = SortOrder.Desc,
        FilteringOptions filteringOptions = FilteringOptions.All,
        string searchTerm = "",
        CancellationToken cancellationToken = default)
    {
        // Start with the minimal base query
        IQueryable<VacancyEntity> query = dataContext.VacancyEntities
            .AsNoTracking()
            .Where(x => x.AccountId == accountId);

        //  Apply shared base filters in a consistent order
        query = ApplyBasicFiltering(query, filteringOptions);

        // Apply closing soon filters if relevant
        query = ApplyClosingSoonFilterByAccountId(query, filteringOptions, accountId);

        // Apply owner-type filtering
        query = filteringOptions switch {
            FilteringOptions.Review => query.Where(x =>
                x.OwnerType == OwnerType.Provider),

            FilteringOptions.AllSharedApplications or FilteringOptions.NewSharedApplications =>
                ApplySharedFilteringByAccountId(query, filteringOptions, accountId)
                    .Where(x => x.OwnerType == OwnerType.Provider),

            FilteringOptions.NewApplications or FilteringOptions.AllApplications =>
                ApplySharedFilteringByAccountId(query, filteringOptions, accountId)
                    .Where(x => x.OwnerType == OwnerType.Employer),

            FilteringOptions.All => query.Where(x =>
                x.OwnerType == OwnerType.Employer ||
                (x.OwnerType == OwnerType.Provider && x.Status == VacancyStatus.Review)),

            _ => query.Where(x => x.OwnerType == OwnerType.Employer)
        };

        // Apply search term
        query = ApplySearchTerm(query, searchTerm);

        // Apply sorting
        if (orderBy is not null)
            query = sortOrder == SortOrder.Desc ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        // Apply paging efficiently
        int skip = (Math.Max(page, (ushort)1) - 1) * pageSize;
        int take = Math.Max(pageSize, (ushort)1);

        int count = await query.CountAsync(cancellationToken);
        var items = await query.Select(c=>new VacancySummaryEntity {
                Id = c.Id,
                Title = c.Title,
                VacancyReference = c.VacancyReference,
                Status = c.Status,
                ClosingDate = c.ClosingDate,
                ApplicationMethod = c.ApplicationMethod,
                ApprenticeshipType = c.ApprenticeshipType,
                CreatedDate = c.CreatedDate,
                LegalEntityName = c.LegalEntityName,
                TransferInfo = c.TransferInfo,
                OwnerType = c.OwnerType,
                HasSubmittedAdditionalQuestions = c.HasSubmittedAdditionalQuestions ?? false,
                Ukprn = c.Ukprn
            })
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);


        return new PaginatedList<VacancySummaryEntity>(items, count, page, pageSize);
    }

    public async Task<PaginatedList<VacancySummaryEntity>> GetManyByUkprnIdAsync<TKey>(int ukprn,
        ushort page = 1,
        ushort pageSize = 25,
        Expression<Func<VacancyEntity, TKey>>? orderBy = null,
        SortOrder sortOrder = SortOrder.Desc,
        FilteringOptions filteringOptions = FilteringOptions.All,
        string searchTerm = "",
        CancellationToken cancellationToken = default)
    {
        // Start with the minimal base query
        IQueryable<VacancyEntity> query = dataContext.VacancyEntities
            .Where(x => x.Ukprn == ukprn && x.OwnerType == OwnerType.Provider);

        // Apply filters
        query = ApplyBasicFiltering(query, filteringOptions);
        query = ApplyClosingSoonFilterByUkprn(query, filteringOptions, ukprn);

        // Apply shared filtering if needed
        if (filteringOptions is FilteringOptions.EmployerReviewedApplications
            or FilteringOptions.AllApplications
            or FilteringOptions.NewApplications)
        {
            query = ApplySharedFilteringByUkprn(query, filteringOptions, ukprn);
        }

        // Apply search term
        query = ApplySearchTerm(query, searchTerm);

        // Apply sorting
        if (orderBy is not null)
        {
            query = sortOrder == SortOrder.Desc
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }

        // Apply paging efficiently
        int skip = (Math.Max(page, (ushort)1) - 1) * pageSize;
        int take = Math.Max(pageSize, (ushort)1);

        int count = await query.CountAsync(cancellationToken);
        var items = await query.Select(c=>new VacancySummaryEntity {
                Id = c.Id,
                Title = c.Title,
                VacancyReference = c.VacancyReference,
                Status = c.Status,
                ClosingDate = c.ClosingDate,
                ApplicationMethod = c.ApplicationMethod,
                ApprenticeshipType = c.ApprenticeshipType,
                CreatedDate = c.CreatedDate,
                LegalEntityName = c.LegalEntityName,
                TransferInfo = c.TransferInfo,
                OwnerType = c.OwnerType,
                HasSubmittedAdditionalQuestions = c.HasSubmittedAdditionalQuestions ?? false,
                Ukprn = c.Ukprn
            })
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return new PaginatedList<VacancySummaryEntity>(items, count, page, pageSize);
    }

    public async Task<VacancyEntity?> GetOneByVacancyReferenceAsync(long vacancyReference, CancellationToken cancellationToken)
    {
        return await dataContext
            .VacancyEntities
            .FirstOrDefaultAsync(x => x.VacancyReference == vacancyReference, cancellationToken);
    }

    public async Task<List<VacancyTransferSummaryEntity>> GetAllTransferInfoByAccountId(long accountId, CancellationToken cancellationToken, bool withTransferInfo = false)
    {
        var employerQuery = dataContext.VacancyEntities
            .AsNoTracking()
            .Select(c=> new VacancyTransferSummaryEntity {
                AccountId = c.AccountId,
                OwnerType = c.OwnerType,
                TransferInfo = c.TransferInfo,
            })
            .Where(v => v.AccountId == accountId && v.OwnerType == OwnerType.Employer)
            .Where(vacancy => !withTransferInfo || vacancy.TransferInfo != null);

        var providerQuery = dataContext.VacancyEntities
            .AsNoTracking()
            .Select(c=> new VacancyTransferSummaryEntity {
                AccountId = c.AccountId,
                OwnerType = c.OwnerType,
                TransferInfo = c.TransferInfo,
                Status = c.Status
            })
            .Where(v => v.AccountId == accountId && v.Status == VacancyStatus.Review && v.OwnerType == OwnerType.Provider)
            .Where(vacancy => !withTransferInfo || vacancy.TransferInfo != null);

        return await employerQuery
            .Union(providerQuery)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<VacancyEntity>> GetAllClosedEmployerVacanciesByClosureReason(long accountId, ClosureReason closureReason, DateTime lastDismissedDate, CancellationToken cancellationToken)
    {
        var vacancies = await dataContext.VacancyEntities
            .AsNoTracking()
            .Where(v => v.AccountId == accountId && v.OwnerType == OwnerType.Employer && v.ClosureReason == closureReason && v.ClosedDate > lastDismissedDate)
            .ToListAsync(cancellationToken);

        return vacancies;
    }

    public async Task<List<VacancyEntity>> GetAllClosedProviderrVacanciesByClosureReason(int ukprn, ClosureReason closureReason, DateTime lastDismissedDate, CancellationToken cancellationToken)
    {
        var vacancies = await dataContext.VacancyEntities
            .AsNoTracking()
            .Where(v => v.Ukprn == ukprn && v.OwnerType == OwnerType.Provider && v.ClosureReason == closureReason && v.ClosedDate > lastDismissedDate)
            .ToListAsync(cancellationToken);

        return vacancies;
    }

    public async Task<List<VacancyTransferSummaryEntity>> GetAllTransferInfoByUkprn(int ukprn, CancellationToken cancellationToken, bool withTransferInfo = false)
    {
        return await dataContext.VacancyEntities
            .AsNoTracking()
            .Select(c=> new VacancyTransferSummaryEntity {
                AccountId = c.AccountId,
                OwnerType = c.OwnerType,
                TransferInfo = c.TransferInfo,
                Status = c.Status,
                Ukprn = c.Ukprn
            })
            .Where(vacancy => vacancy.Ukprn == ukprn && vacancy.OwnerType == OwnerType.Provider)
            .Where(vacancy => !withTransferInfo || vacancy.TransferInfo != null)
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

    public async Task<List<VacancyDashboardCountModel>> GetEmployerDashboard(long accountId, CancellationToken cancellationToken)
    {
        var entity = await dataContext.VacancyEntities
            .AsNoTracking()
            .Where(c => c.AccountId == accountId && c.OwnerType == OwnerType.Employer)
            .Select(c=>
                new {
                    c.Id,
                    c.Status
                })
            .GroupBy(c => c.Status)
            .ToListAsync(cancellationToken);

        return entity.Select((c) => new VacancyDashboardCountModel {
            Status = c.Key,
            Count = c.Count()
        }).ToList();
    }

    public async Task<List<(int, bool)>> GetEmployerVacanciesClosingSoonWithApplications(long accountId,
        CancellationToken cancellationToken)
    {
        var query = from v in dataContext.VacancyEntities
            join ar in dataContext.ApplicationReviewEntities
                on v.VacancyReference equals ar.VacancyReference into arGroup
            from ar in arGroup.DefaultIfEmpty() // Left join
            where v.AccountId == accountId
                  && v.OwnerType == OwnerType.Employer
                  && v.Status == VacancyStatus.Live
                  && v.ClosingDate <= DateTime.UtcNow.AddDays(ClosingSoonDays)
            group ar by v into g
            select new
            {
                HasApplications = g.Any(x => x != null)
            };

        var grouped = await query
            .GroupBy(x => x.HasApplications)
            .Select(g => new { Count = g.Count(), HasApplications = g.Key })
            .ToListAsync(cancellationToken);

        return grouped.Select(g => (g.Count, g.HasApplications)).ToList();
    }

    public async Task<List<VacancyDashboardCountModel>> GetProviderDashboard(int ukprn, CancellationToken cancellationToken)
    {
        var entity = await dataContext.VacancyEntities
            .AsNoTracking()
            .Where(c => c.Ukprn == ukprn && c.OwnerType == OwnerType.Provider)
            .Select(c=>
            new {
                c.Id,
                c.Status
            })
            .GroupBy(c => c.Status)
            .ToListAsync(cancellationToken);

        return entity.Select((c) => new VacancyDashboardCountModel {
            Status = c.Key,
            Count = c.Count()
        }).ToList();
    }
    public async Task<List<(int, bool)>> GetProviderVacanciesClosingSoonWithApplications(int ukprn, CancellationToken cancellationToken)
    {
        var query = from v in dataContext.VacancyEntities
            join ar in dataContext.ApplicationReviewEntities
                on v.VacancyReference equals ar.VacancyReference into arGroup
            from ar in arGroup.DefaultIfEmpty() // Left join
            where v.Ukprn == ukprn
                  && v.OwnerType == OwnerType.Provider
                  && v.Status == VacancyStatus.Live
                  && v.ClosingDate <= DateTime.UtcNow.AddDays(ClosingSoonDays)
            group ar by v into g
            select new
            {
                HasApplications = g.Any(x => x != null)
            };

        var grouped = await query
            .GroupBy(x => x.HasApplications)
            .Select(g => new { Count = g.Count(), HasApplications = g.Key })
            .ToListAsync(cancellationToken);

        return grouped.Select(g => (g.Count, g.HasApplications)).ToList();
    }

    private static IQueryable<VacancyEntity> ApplyBasicFiltering(IQueryable<VacancyEntity> query,
        FilteringOptions filteringOptions)
    {
        return filteringOptions switch {
            // Simple status-based filters
            FilteringOptions.Draft => query.Where(x => x.Status == VacancyStatus.Draft),
            FilteringOptions.Review => query.Where(x => x.Status == VacancyStatus.Review),
            FilteringOptions.Submitted => query.Where(x => x.Status == VacancyStatus.Submitted),
            FilteringOptions.Live => query.Where(x => x.Status == VacancyStatus.Live),
            FilteringOptions.Closed => query.Where(x => x.Status == VacancyStatus.Closed),
            FilteringOptions.Referred => query.Where(x =>
                                            x.Status == VacancyStatus.Referred ||
                                            x.Status == VacancyStatus.Rejected),
            FilteringOptions.Transferred => query.Where(v => v.TransferInfo != null),

            // Application-based filters
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

        searchTerm = searchTerm.Trim();

        // Try parsing the vacancy reference
        bool isValidVacancyReference = long.TryParse(
            searchTerm.Replace("vac", "", StringComparison.CurrentCultureIgnoreCase), out long vacancyReference);

        if (isValidVacancyReference)
        {
            query = query.Where(x => x.VacancyReference == vacancyReference);
            return query;
        }
        
        query = query.Where(v =>
            v.Title!.Contains(searchTerm) || v.LegalEntityName!.Contains(searchTerm)
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
        long accountId) => ApplyClosingSoonFilter(query, filteringOptions, accountId: accountId);

    private IQueryable<VacancyEntity> ApplyClosingSoonFilterByUkprn(
        IQueryable<VacancyEntity> query,
        FilteringOptions filteringOptions,
        int ukprn) => ApplyClosingSoonFilter(query, filteringOptions, ukprn: ukprn);


    private IQueryable<VacancyEntity> ApplyClosingSoonFilter(
        IQueryable<VacancyEntity> query,
        FilteringOptions filteringOptions,
        long? accountId = null,
        int? ukprn = null)
    {
        var closingSoonThreshold = DateTime.UtcNow.AddDays(5);

        var baseFilter = query.Where(x =>
            x.Status == VacancyStatus.Live &&
            x.ClosingDate <= closingSoonThreshold);

        switch (filteringOptions)
        {
            case FilteringOptions.ClosingSoon:
                return baseFilter;

            case FilteringOptions.ClosingSoonWithNoApplications:
                {
                    IQueryable<long> activeApplications;

                    if (accountId.HasValue)
                    {
                        activeApplications = dataContext.ApplicationReviewEntities
                            .AsNoTracking()
                            .Where(a => a.WithdrawnDate == null && a.AccountId == accountId)
                            .Select(a => a.VacancyReference);
                    }
                    else if (ukprn.HasValue)
                    {
                        activeApplications = dataContext.ApplicationReviewEntities
                            .AsNoTracking()
                            .Where(a => a.WithdrawnDate == null && a.Ukprn == ukprn)
                            .Select(a => a.VacancyReference);
                    }
                    else
                    {
                        return baseFilter;
                    }

                    return baseFilter
                        .Where(v =>
                            v.ApplicationMethod == ApplicationMethod.ThroughFindAnApprenticeship &&
                            v.VacancyReference.HasValue &&
                            !activeApplications.Contains(v.VacancyReference.Value));
                }

            default:
                return query;
        }
    }
}