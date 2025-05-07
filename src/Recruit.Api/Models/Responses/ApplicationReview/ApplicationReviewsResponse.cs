namespace SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview
{
    public record ApplicationReviewsResponse(PageInfo PageInfo, IEnumerable<Models.ApplicationReview> ApplicationReviews);
}