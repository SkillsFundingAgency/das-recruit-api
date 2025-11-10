using System.ComponentModel.DataAnnotations;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Models.Requests.Report;

public record PostReportRequest
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    [MaxLength(200)]
    public required string UserId { get; init; }
    [MaxLength(50)]
    public required string CreatedBy { get; init; }
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