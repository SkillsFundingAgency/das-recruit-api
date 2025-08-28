using SFA.DAS.Recruit.Api.Domain;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Domain;

public class WhenGettingEmployerNotificationPreferenceDefaults
{
    [Test, MoqAutoData]
    public void Defaults_Should_Be_Added()
    {
        // arrange
        var prefs = new NotificationPreferences();

        // act
        EmployerNotificationPreferences.UpdateWithDefaults(prefs);

        // assert
        prefs.EventPreferences.Should().HaveCount(4);

        var applicationSubmittedPref = prefs.GetForEvent(NotificationTypes.ApplicationSubmitted);
        applicationSubmittedPref.Method.Should().Be("Email");
        applicationSubmittedPref.Scope.Should().Be(NotificationScope.OrganisationVacancies);
        applicationSubmittedPref.Frequency.Should().Be(NotificationFrequency.Daily);
        
        var vacancyApprovedOrRejectedByDfEPref = prefs.GetForEvent(NotificationTypes.VacancyApprovedOrRejected);
        vacancyApprovedOrRejectedByDfEPref.Method.Should().Be("Email");
        vacancyApprovedOrRejectedByDfEPref.Scope.Should().Be(NotificationScope.OrganisationVacancies);
        vacancyApprovedOrRejectedByDfEPref.Frequency.Should().Be(NotificationFrequency.NotSet);
        
        var vacancySendForReviewPref = prefs.GetForEvent(NotificationTypes.VacancySentForReview);
        vacancySendForReviewPref.Method.Should().Be("Email");
        vacancySendForReviewPref.Scope.Should().Be(NotificationScope.OrganisationVacancies);
        vacancySendForReviewPref.Frequency.Should().Be(NotificationFrequency.NotSet);

        var applicationSharedWithEmployerPref = prefs.GetForEvent(NotificationTypes.ApplicationSharedWithEmployer);
        applicationSharedWithEmployerPref.Method.Should().Be("Email");
        applicationSharedWithEmployerPref.Scope.Should().Be(NotificationScope.OrganisationVacancies);
        applicationSharedWithEmployerPref.Frequency.Should().Be(NotificationFrequency.Immediately);
    }
    
    [Test, MoqAutoData]
    public void Defaults_Should_Not_Override_Existing()
    {
        // arrange
        var existingPref = new NotificationPreference(NotificationTypes.ApplicationSubmitted, "Email", NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately);
        var prefs = new NotificationPreferences
        {
            EventPreferences = [ existingPref ]
        };

        // act
        EmployerNotificationPreferences.UpdateWithDefaults(prefs);

        // assert
        prefs.EventPreferences.Should().HaveCount(4);
        var applicationSubmittedPref = prefs.GetForEvent(NotificationTypes.ApplicationSubmitted);
        applicationSubmittedPref.Should().BeEquivalentTo(existingPref);
    }
    
    [Test, MoqAutoData]
    public void Defaults_Should_Not_Remove_Other_Existing_Preferences_That_Are_Not_Defaults()
    {
        // arrange
        var existingPref = new NotificationPreference(NotificationTypes.VacancyClosingSoon, "Email", NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately);
        var prefs = new NotificationPreferences
        {
            EventPreferences = [ existingPref ]
        };

        // act
        EmployerNotificationPreferences.UpdateWithDefaults(prefs);

        // assert
        prefs.EventPreferences.Should().HaveCount(5);
        var pref = prefs.GetForEvent(NotificationTypes.VacancyClosingSoon);
        pref.Should().Be(existingPref);
    }
}