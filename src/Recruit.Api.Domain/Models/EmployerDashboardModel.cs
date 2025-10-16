namespace SFA.DAS.Recruit.Api.Domain.Models;
public record EmployerDashboardModel : DashboardModel
{
    public EmployerDashboardModel(ApplicationReviewsDashboardModel app, VacancyDashboardModel vac)
        : base(app, vac) { }
}