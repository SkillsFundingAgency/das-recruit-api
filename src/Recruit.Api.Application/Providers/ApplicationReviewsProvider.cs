using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Domain.Entities;
using Recruit.Api.Domain.Models;
using Recruit.Api.Data.Models;

namespace Recruit.Api.Application.Providers;

public interface IApplicationReviewsProvider
{
    Task<ApplicationReviewEntity?> GetById(Guid id, CancellationToken token = default);

    Task<PaginatedList<ApplicationReviewEntity>> GetAllByAccountId(long accountId,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = "CreatedDate",
        bool isAscending = false,
        CancellationToken token = default);

    Task<PaginatedList<ApplicationReviewEntity>> GetAllByUkprn(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = "CreatedDate",
        bool isAscending = false,
        CancellationToken token = default);

    Task<ApplicationReviewEntity?> Update(ApplicationReviewEntity entity, CancellationToken token = default);

    Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default);
}

public class ApplicationReviewsProvider(IApplicationReviewRepository repository) : IApplicationReviewsProvider
{
    public async Task<ApplicationReviewEntity?> GetById(Guid id, CancellationToken token = default)
    {
        return await repository.GetById(id, token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetAllByAccountId(long accountId,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        CancellationToken token = default)
    {
        return await repository.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);
    }

    public async Task<PaginatedList<ApplicationReviewEntity>> GetAllByUkprn(int ukprn,
        int pageNumber = 1,
        int pageSize = 10,
        string sortColumn = "CreatedDate",
        bool isAscending = false, CancellationToken token = default)
    {
        return await repository.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token);
    }

    public async Task<ApplicationReviewEntity?> Update(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        return await repository.Update(entity, token);
    }

    public async Task<UpsertResult<ApplicationReviewEntity>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        return await repository.Upsert(entity, token);
    }
}