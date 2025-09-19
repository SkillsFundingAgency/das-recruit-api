namespace SFA.DAS.Recruit.Api.Domain.Models;

public class NotificationEmail
{
    public required Guid TemplateId { get; set; }
    public required string RecipientAddress { get; set; }
    public required Dictionary<string, string> Tokens { get; set; } = [];
}