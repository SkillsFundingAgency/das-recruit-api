using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.Recruit.Api.Domain.Entities;

[Table("RecruitNotification")]
public class RecruitNotificationEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public required Guid UserId { get; set; }
    public required Guid EmailTemplateId { get; set; }
    public required DateTime SendWhen { get; set; }
    public required string StaticData { get; set; }
    public required string DynamicData { get; set; }
}