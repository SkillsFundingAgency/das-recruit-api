namespace SFA.DAS.Recruit.Api.Domain.Entities;

public class RecruitNotificationEntity
{
    public long Id { get; set; }
    public required Guid UserId { get; set; }
    public required Guid EmailTemplateId { get; set; }
    public required DateTime SendWhen { get; set; }
    public required string StaticData { get; set; }
    public required string DynamicData { get; set; }
    
    // reference properties
    public virtual UserEntity User { get; set; }
}