using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Domain.Models;
public record VacancyDashboardCountModel
{
    public VacancyStatus Status { get; set; }
    public int Count { get; set; }
}