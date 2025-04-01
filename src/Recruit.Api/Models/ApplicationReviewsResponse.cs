using SFA.DAS.Recruit.Api.Models.Responses;

namespace SFA.DAS.Recruit.Api.Models
{
    public record ApplicationReviewsResponse(PageInfo PageInfo, IEnumerable<ApplicationReview> ApplicationReviews);
}