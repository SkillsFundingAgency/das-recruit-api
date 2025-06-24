namespace SFA.DAS.Recruit.Api.Models.Responses.VacancyReview;

public record VacancyReviewsResponse(PageInfo PageInfo, IEnumerable<Models.VacancyReview> VacancyReviews);
