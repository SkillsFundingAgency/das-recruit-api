using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Models.Requests.Report;

public record PostUploadApplicationSummaryReportRequest
{
    public List<ApplicationSummaryReport> Reports { get; init; } = [];
}
