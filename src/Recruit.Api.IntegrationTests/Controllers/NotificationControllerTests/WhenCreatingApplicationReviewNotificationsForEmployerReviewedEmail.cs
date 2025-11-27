using System.Net;
using System.Text.Json;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Testing.Data;
using SFA.DAS.Recruit.Api.Testing.Http;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.NotificationControllerTests;

public class WhenCreatingApplicationReviewNotificationsForEmployerReviewedEmail: BaseFixture
{
    [Test, RecursiveMoqAutoData]
    public async Task And_No_Users_Are_Found_Then_No_Notifications_Are_Created(
        List<ApplicationReviewEntity> applicationReviews,
        List<VacancyEntity> vacancies)
    {
        // arrange
        applicationReviews[1].Status = ApplicationReviewStatus.EmployerUnsuccessful;
        vacancies[0].VacancyReference = applicationReviews[1].VacancyReference;
        
        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet(applicationReviews);
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet(vacancies);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([]);
        Server.DataContext.Setup(x => x.UserEmployerAccountEntities).ReturnsDbSet([]);

        // act
        var response = await Client.PostAsync($"{RouteNames.ApplicationReview}/{applicationReviews[1].Id}/create-notifications", null);
        var notificationEmails = await response.Content.ReadAsAsync<List<NotificationEmail>>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notificationEmails.Should().HaveCount(0);
    }
    
    [Test]
    [RecursiveMoqInlineAutoData(ApplicationReviewStatus.EmployerInterviewing)]
    [RecursiveMoqInlineAutoData(ApplicationReviewStatus.EmployerUnsuccessful)]
    public async Task Then_Notifications_Are_Created(
        ApplicationReviewStatus applicationReviewStatus,
        string expectedHashedAccountId,
        List<UserEntity> users,
        ApplicationReviewEntity applicationReview,
        VacancyEntity vacancy)
    {
        // arrange
        applicationReview.Status = applicationReviewStatus;
        vacancy.OwnerType = OwnerType.Provider;
        vacancy.VacancyReference = applicationReview.VacancyReference;
        var expectedUserNames = users.Take(2).Select(x => x.Name).ToList();
        foreach (var user in users)
        {
            user.Ukprn = vacancy.Ukprn;
            user.UserType = UserType.Provider;
            user.LastSignedInDate = DateTime.UtcNow.AddDays(-1);
        }
        var templateHelper = new EmailTemplateHelper(new DevelopmentEmailTemplateIds(), new DevelopmentRecruitBaseUrls("local"));

        // Make this the originating user
        vacancy.ReviewRequestedByUserId = users[0].Id;
        users[0].NotificationPreferences = new NotificationPreferences {
            EventPreferences = [
                new NotificationPreference(
                    NotificationTypes.SharedApplicationReviewedByEmployer,
                    "",
                    NotificationScope.UserSubmittedVacancies,
                    NotificationFrequency.Immediately)
            ]
        };
        
        // This user wishes to receive all org emails for this event
        users[1].NotificationPreferences = new NotificationPreferences {
            EventPreferences = [
                new NotificationPreference(
                    NotificationTypes.SharedApplicationReviewedByEmployer,
                    "",
                    NotificationScope.OrganisationVacancies,
                    NotificationFrequency.Immediately)
            ]
        };

        // This user isn't interested
        users[2].NotificationPreferences = new NotificationPreferences {
            EventPreferences = [
                new NotificationPreference(
                    NotificationTypes.SharedApplicationReviewedByEmployer,
                    "",
                    NotificationScope.NotSet,
                    NotificationFrequency.Never)
            ]
        };

        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet([applicationReview]);
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet(users);
        
        IEnumerable<RecruitNotificationEntity>? capturedNotifications = null;
        Server.DataContext
            .Setup(x => x.RecruitNotifications.AddRange(It.IsAny<IEnumerable<RecruitNotificationEntity>>()))
            .Callback<IEnumerable<RecruitNotificationEntity>>(x => capturedNotifications = x);

        // act
        var response = await Client.PostAsync($"{RouteNames.ApplicationReview}/{applicationReview.Id}/create-notifications", null);
        var notificationEmails = await response.Content.ReadAsAsync<List<NotificationEmail>>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notificationEmails.Should().HaveCount(0);

        var notifications = capturedNotifications?.ToList();
        notifications.Should().NotBeNull();
        notifications.Should().HaveCount(2);
        notifications.Should().AllSatisfy(x =>
        {
            var staticData = JsonSerializer.Deserialize<Dictionary<string, string>>(x.StaticData);
            staticData.Should().HaveCount(6);
            expectedUserNames.Should().Contain(staticData["firstName"]);
            staticData["employerName"].Should().Be(vacancy.EmployerName!);
            staticData["advertTitle"].Should().Be(vacancy.Title!);
            staticData["vacancyReference"].Should().Be(vacancy.VacancyReference.ToString()!);
            staticData["manageAdvertURL"].Should().EndWith($"/{vacancy.Ukprn}/vacancies/{vacancy.Id}/manage");
            staticData["notificationSettingsURL"].Should().Be(templateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn!.Value.ToString()));
        });
    }
}