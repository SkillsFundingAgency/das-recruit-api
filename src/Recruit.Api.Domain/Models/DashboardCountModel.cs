using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Data.Models;

public class DashboardCountModel
{
    public ApplicationReviewStatus Status { get; set; }
    public int Count { get; set; }
}