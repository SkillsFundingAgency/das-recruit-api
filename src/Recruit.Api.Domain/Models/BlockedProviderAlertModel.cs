namespace SFA.DAS.Recruit.Api.Domain.Models;
public record BlockedProviderAlertModel
{
    public List<string> ClosedVacancies { get; set; } = [];
    public List<string> BlockedProviderNames { get; set; } = [];
}