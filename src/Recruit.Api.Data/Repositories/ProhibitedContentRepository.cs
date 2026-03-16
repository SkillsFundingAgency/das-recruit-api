using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.Repositories;

public interface IProhibitedContentRepository
{
    Task<List<ProhibitedContentEntity>> GetByContentTypeAsync(ProhibitedContentType prohibitedContentType, CancellationToken cancellationToken);
}

internal class ProhibitedContentRepository(IRecruitDataContext dataContext, IMemoryCache memoryCache) : IProhibitedContentRepository
{
    public async Task<List<ProhibitedContentEntity>> GetByContentTypeAsync(ProhibitedContentType prohibitedContentType, CancellationToken cancellationToken)
    {
        if (memoryCache.TryGetValue($"{nameof(ProhibitedContentRepository)}:{prohibitedContentType}",
                out List<ProhibitedContentEntity>? prohibitedContentEntities))
        {
            return prohibitedContentEntities!;
        }
            
        var result = await dataContext.ProhibitedContentEntities
            .AsNoTracking()
            .Where(x => x.ContentType == prohibitedContentType)
            .ToListAsync(cancellationToken);

        memoryCache.Set($"{nameof(ProhibitedContentRepository)}:{prohibitedContentType}", result, TimeSpan.FromDays(1));
        
        return result;
    }
}