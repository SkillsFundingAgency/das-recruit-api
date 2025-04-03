using DtoProhibitedContentType = SFA.DAS.Recruit.Api.Models.ProhibitedContentType;
using DomainProhibitedContentType = SFA.DAS.Recruit.Api.Domain.Models.ProhibitedContentType;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class ProhibitedContentTypeExtensions
{
    public static DomainProhibitedContentType ToDomain(this DtoProhibitedContentType prohibitedContentType)
    {
        return prohibitedContentType switch {
            DtoProhibitedContentType.BannedPhrases => DomainProhibitedContentType.BannedPhrases,
            DtoProhibitedContentType.Profanity => DomainProhibitedContentType.Profanity,
            _ => throw new ArgumentOutOfRangeException(nameof(prohibitedContentType), prohibitedContentType, null)
        };
    }
}