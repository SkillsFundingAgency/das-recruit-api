using Newtonsoft.Json;

namespace SFA.DAS.Recruit.Api.Models.Requests.VacancyAnalytics;

public record PutVacancyAnalyticsRequest
{
    public required List<Domain.Models.VacancyAnalytics> AnalyticsData { get; set; }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(AnalyticsData);
    }
}