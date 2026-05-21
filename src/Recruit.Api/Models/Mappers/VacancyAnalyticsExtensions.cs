using SFA.DAS.Recruit.Api.Models.Responses.VacancyAnalytics;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class VacancyAnalyticsExtensions
{
    public static Domain.Entities.VacancyAnalyticsEntity ToEntity(this Requests.VacancyAnalytics.PutVacancyAnalyticsRequest request, long vacancyReference)
    {
        return new Domain.Entities.VacancyAnalyticsEntity
        {
            VacancyReference = vacancyReference,
            UpdatedDate = DateTime.UtcNow,
            Analytics = request.ToJson()
        };
    }

    public static VacancyAnalyticsResponse ToResponse(this Domain.Entities.VacancyAnalyticsEntity entity)
    {
        return new VacancyAnalyticsResponse
        {
            VacancyReference = entity.VacancyReference,
            UpdatedDate = entity.UpdatedDate,
            Analytics = entity.AnalyticsData.ToList()
        };
    }
}