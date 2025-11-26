namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class VacancyAnalyticsExtensions
{
    public static Domain.Entities.VacancyAnalyticsEntity ToEntity(this Requests.VacancyAnalytics.PutVacancyAnalyticsRequest request, long vacancyReference)
    {
        return new Domain.Entities.VacancyAnalyticsEntity
        {
            VacancyReference = vacancyReference,
            UpdatedDate = DateTime.UtcNow,
            Analytics = request.Analytics.GetRawText()
        };
    }

    public static Responses.VacancyAnalytics.VacancyAnalytics ToResponse(this Domain.Entities.VacancyAnalyticsEntity entity)
    {
        return new Responses.VacancyAnalytics.VacancyAnalytics
        {
            VacancyReference = entity.VacancyReference,
            UpdatedDate = entity.UpdatedDate,
            Analytics = entity.Analytics
        };
    }
}