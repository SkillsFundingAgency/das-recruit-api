namespace SFA.DAS.Recruit.Api.Domain.Models;
public record WithdrawnVacanciesAlertModel
{
    public List<string> ClosedVacancies { get; set; } = [];
}