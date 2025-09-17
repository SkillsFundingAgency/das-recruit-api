using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

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

    public static DateTime GetNextWeeklySendDate(this DateTime when)
    {
        int daysToAdd = ((int) DayOfWeek.Monday - (int) when.DayOfWeek + 7) % 7;
        return when.AddDays(daysToAdd).Date;
    }
    
    public static DateTime GetNextDailySendDate(this DateTime when)
    {
        return when.AddDays(1).Date;
    }
    
    public static string GetCityNames(this List<Address> addresses)
    {
        var cityNames = addresses
            .Select(address => address.GetLastNonEmptyField())
            .Distinct()
            .ToList();

        return cityNames.Count == 1 && addresses.Count > 1
            ? $"{cityNames[0]} ({addresses.Count} available locations)"
            : string.Join(", ", cityNames.OrderBy(x => x));
    }
    
    private static string? GetLastNonEmptyField(this Address address)
    {
        return new[]
        {
            address.AddressLine4,
            address.AddressLine3,
            address.AddressLine2,
            address.AddressLine1,
        }.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
    }
}