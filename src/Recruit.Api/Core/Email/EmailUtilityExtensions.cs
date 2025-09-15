using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;

namespace SFA.DAS.Recruit.Api.Core.Email;

public static class EmailUtilityExtensions
{
    public static List<UserEntity> GetUsersForNotificationType(this List<UserEntity>? users, NotificationTypes notificationType, Guid? originatingUser = null)
    {
        if (users is null)
        {
            return [];
        }

        return users.Where(x =>
        {
            if (!x.NotificationPreferences.TryGetForEvent(notificationType, out var pref))
            {
                return false;
            }
            
            if (pref.Frequency is NotificationFrequency.Never)
            {
                return false;
            }

            switch (pref.Scope)
            {
                case NotificationScope.NotSet:
                case NotificationScope.OrganisationVacancies:
                case NotificationScope.UserSubmittedVacancies when x.Id == originatingUser:
                    return true;
                default:
                    return false;
            }
        }).Distinct().ToList();
    }
}