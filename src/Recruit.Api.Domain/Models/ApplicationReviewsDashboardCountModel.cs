using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Domain.Models;

public record ApplicationReviewsDashboardCountModel
{
    public ApplicationReviewStatus Status { get; init; }
    public int Count { get; init; }
}