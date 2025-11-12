namespace SFA.DAS.Recruit.Api.Domain.Models;
public record ProviderDashboardModel : DashboardModel
{
    public ProviderDashboardModel(ApplicationReviewsDashboardModel app, VacancyDashboardModel vac)
        : base(app, vac) { }
}