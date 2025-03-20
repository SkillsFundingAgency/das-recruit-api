using Recruit.Api.Domain.Enums;

namespace Recruit.Api.Domain.Entities;

public record PatchApplicationReview : ApplicationBase
{
    public static implicit operator PatchApplicationReview(ApplicationReviewEntity entity)
    {
        return new PatchApplicationReview 
        {
            Status = ParseValue<ApplicationStatus>(entity.Status)
        };
    }
}