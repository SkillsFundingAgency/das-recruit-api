namespace SFA.DAS.Recruit.Api.Domain.Models
{
    public record DashboardModel
    {
        public int NewApplicationsCount { get; init; }
        public int EmployerReviewedApplicationsCount { get; init; }
        public int SuccessfulApplicationsCount { get; init; }
        public int UnsuccessfulApplicationsCount { get; init; }
        public int SharedApplicationsCount { get; set; }
        public int AllSharedApplicationsCount { get; set; }
        public bool HasNoApplications { get; init; }

        public int ClosedVacanciesCount { get; init; }
        public int DraftVacanciesCount { get; init; }
        public int ReviewVacanciesCount { get; init; }
        public int ReferredVacanciesCount { get; init; }
        public int LiveVacanciesCount { get; init; }
        public int SubmittedVacanciesCount { get; init; }

        public static DashboardModel MapToDashboardModel(ApplicationReviewsDashboardModel app, VacancyDashboardModel vac)
        {
            return new DashboardModel {
                NewApplicationsCount = app.NewApplicationsCount,
                SharedApplicationsCount = app.SharedApplicationsCount,
                AllSharedApplicationsCount = app.AllSharedApplicationsCount,
                UnsuccessfulApplicationsCount = app.UnsuccessfulApplicationsCount,
                SuccessfulApplicationsCount = app.SuccessfulApplicationsCount,
                EmployerReviewedApplicationsCount = app.EmployerReviewedApplicationsCount,
                HasNoApplications = app.HasNoApplications,

                ClosedVacanciesCount = vac.ClosedVacanciesCount,
                DraftVacanciesCount = vac.DraftVacanciesCount,
                LiveVacanciesCount = vac.LiveVacanciesCount,
                ReviewVacanciesCount = vac.ReviewVacanciesCount,
                ReferredVacanciesCount = vac.ReferredVacanciesCount,
                SubmittedVacanciesCount = vac.SubmittedVacanciesCount,
            };
        }
    }
}