using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

public record GetPagedApplicationReviewsResponse(PageInfo Info, IEnumerable<ApplicationReviewEntity> Items);