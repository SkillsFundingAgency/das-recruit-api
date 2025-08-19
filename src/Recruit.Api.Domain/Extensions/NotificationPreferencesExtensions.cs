using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Domain.Extensions;

public static class NotificationPreferencesExtensions
{
    public static NotificationPreference GetForEvent(this NotificationPreferences notificationPreferences, NotificationTypes eventType)
    {
        return notificationPreferences.EventPreferences.Single(x => x.Event == eventType);
    }
}