using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email;

public class WhenGettingUsersForNotificationTyp
{
    [Test]
    public void Then_Null_List_Is_Handled()
    {
        // act
        var result = EmailUtilityExtensions.GetUsersForNotificationType(null, NotificationTypes.ApplicationSharedWithEmployer);

        // assert
        result.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public void Then_Users_Who_Do_Not_Want_The_Email_Are_Not_Returned(List<UserEntity> users)
    {
        // arrange
        foreach (var user in users)
        {
            user.NotificationPreferences = new NotificationPreferences {
                EventPreferences = [
                    new NotificationPreference(
                        NotificationTypes.SharedApplicationReviewedByEmployer,
                        "",
                        NotificationScope.UserSubmittedVacancies,
                        NotificationFrequency.Never)
                ]
            };
        }
        
        
        // act
        var result = users.GetUsersForNotificationType(NotificationTypes.SharedApplicationReviewedByEmployer);

        // assert
        result.Should().BeEmpty();
    }
    
    [Test, RecursiveMoqAutoData]
    public void Then_The_Originating_User_Is_Notified(List<UserEntity> users)
    {
        // arrange
        foreach (var user in users)
        {
            user.NotificationPreferences = new NotificationPreferences {
                EventPreferences = [
                    new NotificationPreference(
                        NotificationTypes.SharedApplicationReviewedByEmployer,
                        "",
                        NotificationScope.OrganisationVacancies,
                        NotificationFrequency.Never)
                ]
            };
        }
        
        users[1].NotificationPreferences = new NotificationPreferences {
            EventPreferences = [
                new NotificationPreference(
                    NotificationTypes.SharedApplicationReviewedByEmployer,
                    "",
                    NotificationScope.UserSubmittedVacancies,
                    NotificationFrequency.Immediately)
            ]
        };
        
        // act
        var result = users.GetUsersForNotificationType(NotificationTypes.SharedApplicationReviewedByEmployer, users[1].Id);

        // assert
        result.Should().HaveCount(1);
        result.Should().Contain(users[1]);
    }
    
    [Test, RecursiveMoqAutoData]
    public void Then_Users_In_The_Organisation_That_Want_To_Know_About_All_Other_Changes_Are_Notified(List<UserEntity> users)
    {
        // arrange
        foreach (var user in users)
        {
            user.NotificationPreferences = new NotificationPreferences {
                EventPreferences = [
                    new NotificationPreference(
                        NotificationTypes.SharedApplicationReviewedByEmployer,
                        "",
                        NotificationScope.OrganisationVacancies,
                        NotificationFrequency.Immediately)
                ]
            };
        }
        
        users[1].NotificationPreferences = new NotificationPreferences {
            EventPreferences = [
                new NotificationPreference(
                    NotificationTypes.SharedApplicationReviewedByEmployer,
                    "",
                    NotificationScope.UserSubmittedVacancies,
                    NotificationFrequency.Never)
            ]
        };
        
        // act
        var result = users.GetUsersForNotificationType(NotificationTypes.SharedApplicationReviewedByEmployer, users[1].Id);

        // assert
        result.Should().HaveCount(2);
        result.Should().Contain(users[0]);
        result.Should().Contain(users[2]);
    }
    
    [Test, RecursiveMoqAutoData]
    public void Then_The_Default_Behaviour_Is_That_The_User_Should_Be_Notified(List<UserEntity> users)
    {
        // arrange
        foreach (var user in users)
        {
            user.NotificationPreferences = new NotificationPreferences {
                EventPreferences = [
                    new NotificationPreference(
                        NotificationTypes.SharedApplicationReviewedByEmployer,
                        "",
                        NotificationScope.OrganisationVacancies,
                        NotificationFrequency.NotSet)
                ]
            };
        }
        
        // act
        var result = users.GetUsersForNotificationType(NotificationTypes.SharedApplicationReviewedByEmployer);

        // assert
        result.Should().HaveCount(3);
        result.Should().Contain(users[0]);
        result.Should().Contain(users[1]);
        result.Should().Contain(users[2]);
    }
}