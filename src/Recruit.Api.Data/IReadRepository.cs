namespace SFA.DAS.Recruit.Api.Data;

public interface IReadRepository<TEntity, in TKey>
{
    Task<TEntity?> GetOneAsync(TKey key, CancellationToken cancellationToken);
}