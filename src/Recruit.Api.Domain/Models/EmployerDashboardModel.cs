namespace SFA.DAS.Recruit.Api.Domain.Models;
public record EmployerDashboardModel : DashboardModel
{
    public EmployerTransferredVacanciesAlertModel? EmployerRevokedTransferredVacanciesAlert { get; set; }
    public EmployerTransferredVacanciesAlertModel? BlockedProviderTransferredVacanciesAlert { get; set; }
    public BlockedProviderAlertModel? BlockedProviderAlert { get; set; }
    public WithdrawnVacanciesAlertModel? WithDrawnByQaVacanciesAlert { get; set; }
    public EmployerDashboardModel(ApplicationReviewsDashboardModel app, VacancyDashboardModel vac)
        : base(app, vac) { }
}