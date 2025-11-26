namespace SFA.DAS.Recruit.Api.Domain.Entities;
public class VacancyAnalyticsEntity
{
    public required long VacancyReference { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string Analytics { get; set; } = string.Empty;
}