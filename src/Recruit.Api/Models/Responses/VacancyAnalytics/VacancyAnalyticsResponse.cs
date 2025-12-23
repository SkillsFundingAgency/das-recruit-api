namespace SFA.DAS.Recruit.Api.Models.Responses.VacancyAnalytics;

public record VacancyAnalyticsResponse
{
    public required long VacancyReference { get; init; }
    public DateTime UpdatedDate { get; init; }
    public List<Domain.Models.VacancyAnalytics> Analytics { get; init; } = [];
}