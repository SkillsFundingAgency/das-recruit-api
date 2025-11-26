namespace SFA.DAS.Recruit.Api.Models.Responses.VacancyAnalytics;

public record VacancyAnalytics
{
    public required long VacancyReference { get; init; }
    public DateTime UpdatedDate { get; init; }
    public string Analytics { get; init; } = string.Empty;
}