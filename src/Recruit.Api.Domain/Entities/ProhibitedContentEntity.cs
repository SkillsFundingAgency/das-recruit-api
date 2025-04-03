using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Domain.Entities;

public class ProhibitedContentEntity
{
    public ProhibitedContentType ContentType { get; init; }
    public required string Content { get; init; }
}