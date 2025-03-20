using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Domain.Entities;
using Recruit.Api.Domain.Models;
using Recruit.Api.Application.Models.ApplicationReview;

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

    Task<ApplicationReviewEntity?> Update(PatchApplication patchDocument, CancellationToken token = default);

    Task<Tuple<ApplicationReviewEntity, bool>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default);
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

    public async Task<ApplicationReviewEntity?> Update(PatchApplication patchDocument, CancellationToken token = default)
    {
        var entity = await repository.GetById(patchDocument.Id, token);
        if (entity == null) return null;

        var patchedDoc = (PatchApplicationReview)entity;

        patchDocument.Patch.ApplyTo(patchedDoc);
        entity.Status = (short)patchedDoc.Status;
        return await repository.Update(entity, token);
    }

    public async Task<Tuple<ApplicationReviewEntity, bool>> Upsert(ApplicationReviewEntity entity, CancellationToken token = default)
    {
        return await repository.Upsert(entity, token);
    }
}