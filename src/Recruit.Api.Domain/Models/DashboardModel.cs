namespace Recruit.Api.Domain.Models
{
    public record DashboardModel
    {
        public int NewApplicationsCount { get; set; } = 0;
        public int EmployerReviewedApplicationsCount { get; set; } = 0;
    }
}