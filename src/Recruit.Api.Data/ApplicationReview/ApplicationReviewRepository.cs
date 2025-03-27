using Microsoft.EntityFrameworkCore;
using Recruit.Api.Data.Models;
using Recruit.Api.Domain.Entities;
using Recruit.Api.Domain.Extensions;
using Recruit.Api.Domain.Models;

namespace Recruit.Api.Data.ApplicationReview;

public interface IApplicationReviewRepository
{
    Task<ApplicationReviewEntity?> GetById(Guid id, CancellationToken token = default);
    Task<PaginatedList<ApplicationReviewEntity>> GetAllByAccountId(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = "",
        bool isAscending = false,
        CancellationToken token = default);
    Task<PaginatedList<ApplicationReviewEntity>> GetAllByUkprn(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = "",
        bool isAscending = false,
        CancellationToken token = default);
    Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default);
    Task<ApplicationReviewEntity?> Update(ApplicationReviewEntity entity, CancellationToken token = default);
}
public class ApplicationReviewRepository(IRecruitDataContext recruitDataContext) : IApplicationReviewRepository
{
    public async Task<ApplicationReviewEntity?> GetById(Guid id, CancellationToken token = default)
    {
        return await recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(fil => fil.Id == id, token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetAllByAccountId(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = "CreatedDate",
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
        string sortColumn = "",
        bool isAscending = false,
        CancellationToken token = default)
    {
        var query = recruitDataContext.ApplicationReviewEntities
            .AsNoTracking()
            .Where(fil => fil.Ukprn == ukprn);
        return await query.GetPagedAsync(pageNumber, pageSize, sortColumn, isAscending, token);
    }

    public async Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        var applicationReview = await recruitDataContext.ApplicationReviewEntities.FirstOrDefaultAsync(fil => fil.Id == entity.Id, token);
        if (applicationReview == null)
        {
            await recruitDataContext.ApplicationReviewEntities.AddAsync(entity, token);
            await recruitDataContext.SaveChangesAsync(token);
            return UpsertResult.Create(entity, true);
        }
        
        recruitDataContext.Entry(applicationReview).CurrentValues.SetValues(entity);
        await recruitDataContext.SaveChangesAsync(token);
        return UpsertResult.Create(entity, false);
    }

    public async Task<ApplicationReviewEntity?> Update(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        var applicationReview = await recruitDataContext.ApplicationReviewEntities.FirstOrDefaultAsync(fil => fil.Id == entity.Id, token);
        if (applicationReview is null)
        {
            return null;
        }
        
        recruitDataContext.Entry(applicationReview).CurrentValues.SetValues(entity);
        await recruitDataContext.SaveChangesAsync(token);
        return entity;
    }
}