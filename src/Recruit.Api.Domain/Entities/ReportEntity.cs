using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Domain.Entities;
public class ReportEntity
{
    public required Guid Id { get; set; }
    [MaxLength(200)]
    public required string UserId { get; set; }
    [MaxLength(50)]
    public string? CreatedBy { get; set; }
    public required string Name { get; set; }
    public ReportType Type { get; set; }
    public ReportOwnerType OwnerType { get; set; }
    public required DateTime CreatedDate { get; set; }
    public int DownloadCount { get; set; } = 0;
    [MaxLength(1000)] 
    public required string DynamicCriteria { get; set; } = null!;
    private bool TryGetCriteria(out ReportCriteria? criteria)
    {
        try
        {
            criteria = JsonSerializer.Deserialize<ReportCriteria>(DynamicCriteria);
            return criteria is not null;
        }
        catch
        {
            criteria = null;
            return false;
        }
    }

    [NotMapped]
    public ReportCriteria? Criteria
    {
        get
        {
            _ = TryGetCriteria(out var criteria);
            return criteria;
        }
    }
}