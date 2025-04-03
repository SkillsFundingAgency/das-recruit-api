using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.ProhibitedContent;

public interface IProhibitedContentRepository
{
    Task<List<ProhibitedContentEntity>> GetByContentTypeAsync(ProhibitedContentType prohibitedContentType, CancellationToken cancellationToken);
}

public class ProhibitedContentRepository(IRecruitDataContext dataContext) : IProhibitedContentRepository
{
    public Task<List<ProhibitedContentEntity>> GetByContentTypeAsync(ProhibitedContentType prohibitedContentType, CancellationToken cancellationToken)
    {
        return dataContext.ProhibitedContentEntities
            .AsNoTracking()
            .Where(x => x.ContentType == prohibitedContentType)
            .ToListAsync(cancellationToken);
    }
}