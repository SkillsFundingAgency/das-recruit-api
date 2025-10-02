namespace SFA.DAS.Recruit.Api.Domain.Models
{
    public record DashboardModel
    {
        public int NewApplicationsCount { get; }
        public int EmployerReviewedApplicationsCount { get; }
        public int SuccessfulApplicationsCount { get; }
        public int UnsuccessfulApplicationsCount { get; }
        public int SharedApplicationsCount { get; }
        public int AllSharedApplicationsCount { get; }
        public bool HasNoApplications { get; }

        public int ClosedVacanciesCount { get; }
        public int DraftVacanciesCount { get; }
        public int ReviewVacanciesCount { get; }
        public int ReferredVacanciesCount { get; }
        public int LiveVacanciesCount { get; }
        public int SubmittedVacanciesCount { get; }

        protected DashboardModel(
            ApplicationReviewsDashboardModel app,
            VacancyDashboardModel vac)
        {
            NewApplicationsCount = app.NewApplicationsCount;
            SharedApplicationsCount = app.SharedApplicationsCount;
            AllSharedApplicationsCount = app.AllSharedApplicationsCount;
            UnsuccessfulApplicationsCount = app.UnsuccessfulApplicationsCount;
            SuccessfulApplicationsCount = app.SuccessfulApplicationsCount;
            EmployerReviewedApplicationsCount = app.EmployerReviewedApplicationsCount;
            HasNoApplications = app.HasNoApplications;

            ClosedVacanciesCount = vac.ClosedVacanciesCount;
            DraftVacanciesCount = vac.DraftVacanciesCount;
            LiveVacanciesCount = vac.LiveVacanciesCount;
            ReviewVacanciesCount = vac.ReviewVacanciesCount;
            ReferredVacanciesCount = vac.ReferredVacanciesCount;
            SubmittedVacanciesCount = vac.SubmittedVacanciesCount;
        }
    }
}