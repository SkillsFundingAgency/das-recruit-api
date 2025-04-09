using SFA.DAS.Recruit.Api.Data.Models;

namespace SFA.DAS.Recruit.Api.Data;

public interface IWriteRepository<TEntity, in TKey>
{
    Task<UpsertResult<TEntity>> UpsertAsync(TEntity entity, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(TKey key, CancellationToken cancellationToken);
}