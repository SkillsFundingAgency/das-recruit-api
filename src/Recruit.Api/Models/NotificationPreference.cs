namespace SFA.DAS.Recruit.Api.Models;

public class NotificationPreferences
{
    public List<NotificationPreference> EventPreferences { get; set; } = [];
}

public record NotificationPreference(string Event, string Method, string Scope, string Frequency);