using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Domain.Extensions;

public static class NotificationPreferencesExtensions
{
    public static NotificationPreference GetForEvent(this NotificationPreferences notificationPreferences, NotificationTypes eventType)
    {
        return notificationPreferences.EventPreferences.Single(x => x.Event == eventType);
    }
    
    public static bool TryGetForEvent(this NotificationPreferences notificationPreferences, NotificationTypes eventType, out NotificationPreference? notificationPreference)
    {
        notificationPreference = notificationPreferences.EventPreferences.FirstOrDefault(x => x.Event == eventType);
        return notificationPreference is not null;
    }
}