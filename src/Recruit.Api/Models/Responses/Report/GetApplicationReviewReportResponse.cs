using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Models.Responses.Report;

public record GetApplicationReviewReportResponse
{
    public List<ApplicationReviewReport> ApplicationReviewReports { get; set; } = [];
}
