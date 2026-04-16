using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Models.Responses.Report;

public record GetQaReportResponse
{
    public List<QaReport> QaReports { get; set; } = [];
}
