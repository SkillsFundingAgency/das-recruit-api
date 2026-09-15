using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Models.Responses.Report;

public record GetApplicationSummaryReportResponse
{
    public List<ApplicationSummaryReport> Reports { get; set; } = [];
}
