namespace SFA.DAS.Recruit.Api.Models;

public class RecruitNotification
{
    public long? Id { get; set; }
    public required Guid UserId { get; set; }
    public required Guid EmailTemplateId { get; set; }
    public required DateTime SendWhen { get; set; }
    public required Dictionary<string, string> StaticData { get; set; }
    public required Dictionary<string, string> DynamicData { get; set; }
}