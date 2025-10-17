namespace SFA.DAS.Recruit.Api.Domain.Models;
public record EmployerAlertsModel
{
    public EmployerTransferredVacanciesAlertModel? EmployerRevokedTransferredVacanciesAlert { get; set; } = new();
    public EmployerTransferredVacanciesAlertModel? BlockedProviderTransferredVacanciesAlert { get; set; } = new();
    public BlockedProviderAlertModel? BlockedProviderAlert { get; set; } = new();
    public WithdrawnVacanciesAlertModel? WithDrawnByQaVacanciesAlert { get; set; } = new();
}