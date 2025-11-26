using System.Text.Json;

namespace SFA.DAS.Recruit.Api.Models.Requests.VacancyAnalytics;

public record PutVacancyAnalyticsRequest
{
    public required JsonElement Analytics { get; set; }
}
