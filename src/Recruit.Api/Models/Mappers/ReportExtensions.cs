using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Requests.Report;
using SFA.DAS.Recruit.Api.Models.Responses.Report;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class ReportExtensions
{
    public static GetApplicationReviewReportResponse ToGetResponse(this List<Domain.Models.ApplicationReviewReport> reports)
    {
        return new GetApplicationReviewReportResponse
        {
            ApplicationReviewReports = reports
        };
    }

    public static ReportEntity ToEntity(this PostReportRequest request)
    {
        return new ReportEntity
        {
            CreatedDate = DateTime.UtcNow,
            CreatedBy = request.CreatedBy,
            Name = request.Name,
            OwnerType = request.OwnerType,
            Type = request.Type,
            UserId = request.UserId,
            Id = request.Id,
            DynamicCriteria = System.Text.Json.JsonSerializer.Serialize(new Domain.Models.ReportCriteria
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Ukprn = request.Ukprn
            })
        };
    }

    public static Report ToResponse(this ReportEntity entity)
    {
        return new Report
        {
            Id = entity.Id,
            Name = entity.Name,
            OwnerType = entity.OwnerType,
            Type = entity.Type,
            CreatedDate = entity.CreatedDate,
            UserId = entity.UserId,
            CreatedBy = entity.CreatedBy,
            DynamicCriteria = entity.DynamicCriteria
        };
    }
}