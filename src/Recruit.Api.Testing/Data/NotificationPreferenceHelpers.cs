using SFA.DAS.Recruit.Api.Domain;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Testing.Data;

public static class NotificationPreferenceHelpers
{
    public static void SetEmailPref(
        this UserEntity user,
        NotificationTypes notificationType,
        NotificationScope scope,
        NotificationFrequency frequency)
    {
        ArgumentNullException.ThrowIfNull(user);
        user.NotificationPreferences = new NotificationPreferences();
        NotificationPreferenceDefaults.Update(user);
        if (user.NotificationPreferences.TryGetForEvent(notificationType, out var pref))
        {
            user.NotificationPreferences.EventPreferences.Remove(pref);
        }
        user.NotificationPreferences.EventPreferences.Add(new NotificationPreference(notificationType, string.Empty, scope, frequency));
    }
}