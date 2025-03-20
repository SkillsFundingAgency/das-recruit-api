using Recruit.Api.Domain.Enums;

namespace Recruit.Api.Domain.Entities;

public record ApplicationBase
{
    public ApplicationStatus Status { get; init; }
        
    protected static T ParseValue<T>(short status) where T : struct, Enum
    {
        Enum.TryParse<T>(status.ToString(), true, out var sectionStatus);
        return sectionStatus;
    }
}