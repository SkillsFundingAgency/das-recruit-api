using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Models;

public record Report
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public string Name { get; set; } = null!;
    public ReportType Type { get; set; }
    public ReportOwnerType OwnerType { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public int DownloadCount { get; set; } = 0;
    public string DynamicCriteria { get; set; } = null!;
}