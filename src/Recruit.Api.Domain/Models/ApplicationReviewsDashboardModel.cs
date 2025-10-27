using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Domain.Models
{
    public record ApplicationReviewsDashboardModel
    {
        public int NewApplicationsCount { get; init; } = 0;
        public int EmployerReviewedApplicationsCount { get; init; } = 0;
        public int SuccessfulApplicationsCount { get; init; } = 0;
        public int UnsuccessfulApplicationsCount { get; init; } = 0;
        public int SharedApplicationsCount { get; set; } = 0;
        public int AllSharedApplicationsCount { get; set; } = 0;
        public bool HasNoApplications { get; init; } = false;

        public static implicit operator ApplicationReviewsDashboardModel(List<ApplicationReviewsDashboardCountModel> source)
        {
            return new ApplicationReviewsDashboardModel 
            {
                NewApplicationsCount = source.FirstOrDefault(c => c.Status == ApplicationReviewStatus.New)?.Count ?? 0,
                SharedApplicationsCount = 0,
                AllSharedApplicationsCount = 0,
                SuccessfulApplicationsCount = source.FirstOrDefault(c => c.Status == ApplicationReviewStatus.Successful)?.Count ?? 0,
                UnsuccessfulApplicationsCount = source.FirstOrDefault(c => c.Status == ApplicationReviewStatus.Unsuccessful)?.Count ?? 0,
                EmployerReviewedApplicationsCount = (source.FirstOrDefault(c => c.Status == ApplicationReviewStatus.EmployerInterviewing)?.Count ?? 0)
                                                    + (source.FirstOrDefault(c => c.Status == ApplicationReviewStatus.EmployerUnsuccessful)?.Count ?? 0)
            };
        }
    }
}