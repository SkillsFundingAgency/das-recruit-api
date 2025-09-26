using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
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
            .Where(x => x.AccountId == accountId);

        query = ApplyFiltering(query, filteringOptions, closingSoonThreshold);
        query = ApplySearchTerm(query, searchTerm);

        int count = await query.CountAsync(cancellationToken);
        if (orderBy is not null)
        {
            query = sortOrder is SortOrder.Desc
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }

        int skip = (Math.Max(page, (ushort)1) - 1) * pageSize;
        ushort take = Math.Max(pageSize, (ushort)1);

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
            .Where(x => x.Ukprn == ukprn);

        query = ApplyFiltering(query, filteringOptions, closingSoonThreshold);
        query = ApplySearchTerm(query, searchTerm);

        int count = await query.CountAsync(cancellationToken);
        if (orderBy is not null)
        {
            query = sortOrder is SortOrder.Desc
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }

        int skip = (Math.Max(page, (ushort)1) - 1) * pageSize;
        ushort take = Math.Max(pageSize, (ushort)1);

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
        return await dataContext.VacancyEntities
            .AsNoTracking()
            .Where(vacancy => vacancy.AccountId == accountId && vacancy.OwnerType == OwnerType.Employer)
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

    private static IQueryable<VacancyEntity> ApplyFiltering(IQueryable<VacancyEntity> query,
        FilteringOptions filteringOptions,
        DateTime closingSoonThreshold)
    {
        return filteringOptions switch {
            FilteringOptions.Draft => query.Where(x => x.Status == VacancyStatus.Draft),
            FilteringOptions.Review => query.Where(x => x.Status == VacancyStatus.Review),
            FilteringOptions.Submitted => query.Where(x => x.Status == VacancyStatus.Submitted),
            FilteringOptions.Live => query.Where(x => x.Status == VacancyStatus.Live),
            FilteringOptions.Closed => query.Where(x => x.Status == VacancyStatus.Closed),
            FilteringOptions.Referred => query.Where(x => x.Status == VacancyStatus.Referred || x.Status == VacancyStatus.Rejected),
            FilteringOptions.NewApplications or FilteringOptions.AllApplications
                                           => query.Where(x => x.Status == VacancyStatus.Live || x.Status == VacancyStatus.Closed),
            FilteringOptions.ClosingSoon => query.Where(x => x.Status == VacancyStatus.Live && x.ClosingDate < closingSoonThreshold),
            FilteringOptions.ClosingSoonWithNoApplications
                                           => query.Where(x =>
                                               x.Status == VacancyStatus.Live &&
                                               x.ClosingDate < closingSoonThreshold &&
                                               x.ApplicationMethod == ApplicationMethod.ThroughFindAnApprenticeship),
            FilteringOptions.Transferred => query
                .AsEnumerable()
                .Where(v =>
                {
                    var info = ApiUtils.DeserializeOrNull<TransferInfo>(v.TransferInfo);
                    return info?.TransferredDate != null;
                })
                .AsQueryable(),
            FilteringOptions.NewSharedApplications or FilteringOptions.AllSharedApplications
                                           => query.Where(x =>
                                               (x.Status == VacancyStatus.Live || x.Status == VacancyStatus.Closed) &&
                                               x.OwnerType == OwnerType.Provider),
            FilteringOptions.Dashboard => query.Where(x => x.Status == VacancyStatus.Live || x.Status == VacancyStatus.Closed),
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
}