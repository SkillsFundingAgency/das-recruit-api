namespace SFA.DAS.Recruit.Api.Domain.Models
{
    public record DashboardModel
    {
        public int NewApplicationsCount { get; init; } = 0;
        public int EmployerReviewedApplicationsCount { get; init; } = 0;
    }
}