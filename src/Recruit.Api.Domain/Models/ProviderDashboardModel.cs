namespace SFA.DAS.Recruit.Api.Domain.Models;
public record ProviderDashboardModel : DashboardModel
{
    public ProviderTransferredVacanciesAlertModel ProviderTransferredVacanciesAlert { get; set; } = new();
    public WithdrawnVacanciesAlertModel WithdrawnVacanciesAlert { get; set; } = new();

    public ProviderDashboardModel(ApplicationReviewsDashboardModel app, VacancyDashboardModel vac)
        : base(app, vac) { }
}