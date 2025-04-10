using SFA.DAS.Recruit.Api.Data.Models;

namespace SFA.DAS.Recruit.Api.Data;

public interface IWriteRepository<TEntity, in TKey>
{
    Task<UpsertResult<TEntity>> UpsertOneAsync(TEntity entity, CancellationToken cancellationToken);
    Task<bool> DeleteOneAsync(TKey key, CancellationToken cancellationToken);
}