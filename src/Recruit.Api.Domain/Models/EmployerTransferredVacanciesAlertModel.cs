namespace SFA.DAS.Recruit.Api.Domain.Models;
public record EmployerTransferredVacanciesAlertModel
{
    public int TransferredVacanciesCount { get; init; }
    public List<string?> TransferredVacanciesProviderNames { get; init; } = [];
}