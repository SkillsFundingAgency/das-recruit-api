using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Domain;

public static class NotificationPreferenceDefaults
{
    public static void Update(UserEntity? user)
    {
        if (user is null)
        {
            return;
        }
        user.NotificationPreferences ??= new NotificationPreferences();
        
        switch (user.UserType)
        {
            case UserType.Employer:
                EmployerNotificationPreferences.UpdateWithDefaults(user.NotificationPreferences);
                break;
            case UserType.Provider:
                ProviderNotificationPreferences.UpdateWithDefaults(user.NotificationPreferences);
                break;
            default:
                throw new ArgumentOutOfRangeException($"Cannot set notification preference defaults, unknown user type '{user.UserType}'");
        }
    }

    public static void Update(List<UserEntity>? users)
    {
        users?.ForEach(Update);
    }
}

internal static class EmployerNotificationPreferences
{
    private const string Channel = "Email"; 
    
    private static readonly List<NotificationPreference> EmployerDefaults = [
        new (NotificationTypes.ApplicationSubmitted, Channel, NotificationScope.OrganisationVacancies, NotificationFrequency.Daily),
        new (NotificationTypes.VacancyApprovedOrRejected, Channel, NotificationScope.OrganisationVacancies, NotificationFrequency.NotSet),
        new (NotificationTypes.VacancySentForReview, Channel, NotificationScope.OrganisationVacancies, NotificationFrequency.NotSet),
        new (NotificationTypes.ApplicationSharedWithEmployer, Channel, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately),
    ];

    public static void UpdateWithDefaults(NotificationPreferences preferences)
    {
        var items = preferences.EventPreferences;
        var defaultsToAdd = EmployerDefaults.Where(x => preferences.EventPreferences.All(y => y.Event != x.Event)).Select(x => x with {});
        items.AddRange(defaultsToAdd);
    }
}

internal static class ProviderNotificationPreferences
{
    private const string Channel = "Email"; 
    
    private static readonly List<NotificationPreference> ProviderDefaults = [
        new (NotificationTypes.ApplicationSubmitted, Channel, NotificationScope.OrganisationVacancies, NotificationFrequency.Daily),
        new (NotificationTypes.VacancyApprovedOrRejected, Channel, NotificationScope.OrganisationVacancies, NotificationFrequency.NotSet),
        new (NotificationTypes.SharedApplicationReviewedByEmployer, Channel, NotificationScope.OrganisationVacancies, NotificationFrequency.Daily),
        new (NotificationTypes.ProviderAttachedToVacancy, Channel, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately),
    ];

    public static void UpdateWithDefaults(NotificationPreferences preferences)
    {
        var items = preferences.EventPreferences;
        var defaultsToAdd = ProviderDefaults.Where(x => preferences.EventPreferences.All(y => y.Event != x.Event)).Select(x => x with {});
        items.AddRange(defaultsToAdd);
    }
}