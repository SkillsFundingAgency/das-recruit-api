using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Data.Repositories;

public interface IBlockedOrganisationRepository : IReadRepository<BlockedOrganisationEntity, Guid>, IWriteRepository<BlockedOrganisationEntity, Guid>
{
    Task<List<BlockedOrganisationEntity>> GetByOrganisationTypeAsync(string organisationType, CancellationToken cancellationToken);
    Task<BlockedOrganisationEntity?> GetLatestByOrganisationIdAsync(string organisationId, CancellationToken cancellationToken);
}

internal class BlockedOrganisationRepository(IRecruitDataContext dataContext) : IBlockedOrganisationRepository
{
    public Task<BlockedOrganisationEntity?> GetOneAsync(Guid id, CancellationToken cancellationToken)
    {
        return dataContext.BlockedOrganisationEntities
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<UpsertResult<BlockedOrganisationEntity>> UpsertOneAsync(BlockedOrganisationEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await GetOneAsync(entity.Id, cancellationToken);
        if (existingEntity is null)
        {
            await dataContext.BlockedOrganisationEntities.AddAsync(entity, cancellationToken);
            await dataContext.SaveChangesAsync(cancellationToken);
            return UpsertResult.Create(entity, true);
        }

        dataContext.SetValues(existingEntity, entity);
        await dataContext.SaveChangesAsync(cancellationToken);
        return UpsertResult.Create(entity, false);
    }

    public async Task<bool> DeleteOneAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetOneAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        dataContext.BlockedOrganisationEntities.Remove(entity);
        await dataContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<List<BlockedOrganisationEntity>> GetByOrganisationTypeAsync(string organisationType, CancellationToken cancellationToken)
    {
        return dataContext.BlockedOrganisationEntities
            .Where(x => x.OrganisationType == organisationType)
            .OrderByDescending(x => x.UpdatedDate)
            .ToListAsync(cancellationToken);
    }

    public Task<BlockedOrganisationEntity?> GetLatestByOrganisationIdAsync(string organisationId, CancellationToken cancellationToken)
    {
        return dataContext.BlockedOrganisationEntities
            .Where(x => x.OrganisationId == organisationId)
            .OrderByDescending(x => x.UpdatedDate)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
