using SFA.DAS.Recruit.Api.Domain;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Testing;

namespace SFA.DAS.Recruit.Api.UnitTests.Domain;

public class WhenGettingNotificationPreferenceDefaults
{
    [Test, RecursiveMoqAutoData]
    public void User_Is_Updated_With_Employer_Defaults(UserEntity user)
    {
        // arrange
        user.UserType = UserType.Employer;
        var prefs = user.NotificationPreferences.JsonClone();
        EmployerNotificationPreferences.UpdateWithDefaults(prefs);

        // act
        NotificationPreferenceDefaults.Update(user);

        // assert
        user.NotificationPreferences.Should().BeEquivalentTo(prefs);
    }
    
    [Test, RecursiveMoqAutoData]
    public void User_Is_Updated_With_Provider_Defaults(UserEntity user)
    {
        // arrange
        user.UserType = UserType.Provider;
        var prefs = user.NotificationPreferences.JsonClone();
        ProviderNotificationPreferences.UpdateWithDefaults(prefs);

        // act
        NotificationPreferenceDefaults.Update(user);

        // assert
        user.NotificationPreferences.Should().BeEquivalentTo(prefs);
    }
    
    [Test, RecursiveMoqAutoData]
    public void Users_Are_Updated_With_Defaults(List<UserEntity> users)
    {
        // arrange
        users[0].UserType = UserType.Provider;
        users[1].UserType = UserType.Employer;
        
        var prefs1 = users[0].NotificationPreferences.JsonClone();
        var prefs2 = users[1].NotificationPreferences.JsonClone();

        ProviderNotificationPreferences.UpdateWithDefaults(prefs1);
        EmployerNotificationPreferences.UpdateWithDefaults(prefs2);

        // act
        NotificationPreferenceDefaults.Update(users);

        // assert
        users[0].NotificationPreferences.Should().BeEquivalentTo(prefs1);
        users[1].NotificationPreferences.Should().BeEquivalentTo(prefs2);
    }
}