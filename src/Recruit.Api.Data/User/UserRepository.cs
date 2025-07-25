using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.User;

public interface IUserRepository : IReadRepository<UserEntity, Guid>, IWriteRepository<UserEntity, Guid>;

public class UserRepository(IRecruitDataContext dataContext) : IUserRepository
{
    public Task<UserEntity?> GetOneAsync(Guid key, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<UpsertResult<UserEntity>> UpsertOneAsync(UserEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await dataContext.UserEntities.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
        if (existingEntity is null)
        {
            await dataContext.UserEntities.AddAsync(entity, cancellationToken);
            await dataContext.SaveChangesAsync(cancellationToken);
            return UpsertResult.Create(entity, true);
        }
        entity.UpdatedDate = DateTime.UtcNow;
        dataContext.SetValues(existingEntity, entity);
        await dataContext.SaveChangesAsync(cancellationToken);
        return UpsertResult.Create(entity, false);
    }


    public Task<bool> DeleteOneAsync(Guid key, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}