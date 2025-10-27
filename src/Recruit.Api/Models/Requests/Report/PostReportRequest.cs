using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Models.Requests.Report;

public record PostReportRequest
{
    public string Name { get; init; } = null!;
    public Guid UserId { get; init; }
    public string CreatedBy { get; init; } = null!;
    public required DateTime FromDate { get; init; }
    public required DateTime ToDate { get; init; }
    public int? Ukprn { get; init; }
    public required ReportOwnerType OwnerType { get; init; }
    public ReportType Type
    {
        get
        {
            return OwnerType is ReportOwnerType.Provider ? ReportType.ProviderApplications : ReportType.QaApplications;
        }
    }
}