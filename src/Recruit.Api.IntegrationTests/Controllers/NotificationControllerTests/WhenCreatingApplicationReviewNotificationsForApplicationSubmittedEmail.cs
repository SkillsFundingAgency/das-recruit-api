using System.Net;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.UnitTests;
using Address = SFA.DAS.Recruit.Api.Domain.Models.Address;
using UserType = SFA.DAS.Recruit.Api.Domain.Enums.UserType;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.NotificationControllerTests;

public class WhenCreatingApplicationReviewNotificationsForApplicationSubmittedEmail : BaseFixture
{
    [Test, MoqAutoData]
    public async Task And_No_Users_Are_Found_Then_No_Notifications_Are_Created(
        List<ApplicationReviewEntity> applicationReviews,
        List<VacancyEntity> vacancies)
    {
        // arrange
        applicationReviews[1].Status = ApplicationReviewStatus.New;
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
    
    [Test, RecruitAutoData]
    public async Task Then_Immediate_Notifications_Are_Returned_For_Employer_User(
        string expectedHashedAccountId,
        ApplicationReviewEntity applicationReview,
        VacancyEntity vacancy,
        UserEntity providerUser,
        UserEntity employerUser)
    {
        // arrange
        applicationReview.Status = ApplicationReviewStatus.New;
        vacancy.OwnerType = OwnerType.Employer;
        vacancy.VacancyReference = applicationReview.VacancyReference;
        vacancy.EmployerLocationOption = AvailableWhere.AcrossEngland;

        providerUser.UserType = UserType.Provider;
        providerUser.Ukprn = vacancy.Ukprn;
        providerUser.SetEmailPref(NotificationTypes.ApplicationSubmitted, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately);
        
        employerUser.UserType = UserType.Employer;
        employerUser.EmployerAccounts = [
            new UserEmployerAccountEntity {
                UserId = employerUser.Id,
                EmployerAccountId = applicationReview.AccountId,
                User = employerUser
            }
        ];
        employerUser.SetEmailPref(NotificationTypes.ApplicationSubmitted, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately);
        var templateHelper = new EmailTemplateHelper(new DevelopmentEmailTemplateIds(), new DevelopmentRecruitBaseUrls("local"));
        
        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet([applicationReview]);
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([employerUser, providerUser]);
        Server.DataContext.Setup(x => x.UserEmployerAccountEntities).ReturnsDbSet(employerUser.EmployerAccounts);
        Server.EncodingService
            .Setup(x => x.Encode(applicationReview.AccountId, EncodingType.AccountId))
            .Returns(expectedHashedAccountId);
    
        // act
        var response = await Client.PostAsync($"{RouteNames.ApplicationReview}/{applicationReview.Id}/create-notifications", null);
        var notificationEmails = await response.Content.ReadAsAsync<List<NotificationEmail>>();
    
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notificationEmails.Should().HaveCount(1);
        notificationEmails.Should().AllSatisfy(x =>
        {
            x.Tokens.Should().HaveCount(7);
            x.RecipientAddress.Should().Be(employerUser.Email);
            x.Tokens["firstName"].Should().Be(employerUser.Name);
            x.Tokens["advertTitle"].Should().Be(vacancy.Title!);
            x.Tokens["employerName"].Should().Be(vacancy.EmployerName);
            x.Tokens["vacancyReference"].Should().Be(vacancy.VacancyReference.ToString());
            x.Tokens["notificationSettingsURL"].Should().Be(templateHelper.EmployerManageNotificationsUrl(expectedHashedAccountId));
            x.Tokens["manageAdvertURL"].Should().Be($"{templateHelper.RecruitEmployerBaseUrl}/accounts/{expectedHashedAccountId}/vacancies/{vacancy.Id}/manage");
            x.Tokens["location"].Should().Be("Recruiting nationally");
        });
    }
    
    [Test, RecruitAutoData]
    public async Task Then_Immediate_Notifications_Are_Returned_For_Provider_User(
        ApplicationReviewEntity applicationReview,
        VacancyEntity vacancy,
        UserEntity providerUser,
        UserEntity employerUser)
    {
        // arrange
        applicationReview.Status = ApplicationReviewStatus.New;
        vacancy.OwnerType = OwnerType.Provider;
        vacancy.VacancyReference = applicationReview.VacancyReference;
        vacancy.EmployerLocationOption = AvailableWhere.AcrossEngland;
        var templateHelper = new EmailTemplateHelper(new DevelopmentEmailTemplateIds(), new DevelopmentRecruitBaseUrls("local"));

        providerUser.UserType = UserType.Provider;
        providerUser.Ukprn = vacancy.Ukprn;
        providerUser.SetEmailPref(NotificationTypes.ApplicationSubmitted, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately);
        
        employerUser.UserType = UserType.Employer;
        employerUser.EmployerAccounts = [
            new UserEmployerAccountEntity {
                UserId = employerUser.Id,
                EmployerAccountId = applicationReview.AccountId,
                User = employerUser
            }
        ];
        employerUser.SetEmailPref(NotificationTypes.ApplicationSubmitted, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately); 
        
        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet([applicationReview]);
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([employerUser, providerUser]);
        Server.DataContext.Setup(x => x.UserEmployerAccountEntities).ReturnsDbSet(employerUser.EmployerAccounts);
    
        // act
        var response = await Client.PostAsync($"{RouteNames.ApplicationReview}/{applicationReview.Id}/create-notifications", null);
        var notificationEmails = await response.Content.ReadAsAsync<List<NotificationEmail>>();
    
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notificationEmails.Should().HaveCount(1);
        notificationEmails.Should().AllSatisfy(x =>
        {
            x.RecipientAddress.Should().Be(providerUser.Email);
            x.Tokens.Should().HaveCount(7);
            x.Tokens["firstName"].Should().Be(providerUser.Name);
            x.Tokens["advertTitle"].Should().Be(vacancy.Title!);
            x.Tokens["employerName"].Should().Be(vacancy.EmployerName);
            x.Tokens["vacancyReference"].Should().Be(vacancy.VacancyReference.ToString());
            x.Tokens["notificationSettingsURL"].Should().Be(templateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn!.Value.ToString()));
            x.Tokens["manageAdvertURL"].Should().Be($"{templateHelper.RecruitProviderBaseUrl}/{vacancy.Ukprn}/vacancies/{vacancy.Id}/manage");
            x.Tokens["location"].Should().Be("Recruiting nationally");
        });
    }
    
    [Test, RecruitAutoData]
    public async Task The_The_Location_Text_Is_Generated_For_Multiple_Cities(
        ApplicationReviewEntity applicationReview,
        VacancyEntity vacancy,
        UserEntity providerUser,
        List<Address> addresses)
    {
        // arrange
        applicationReview.Status = ApplicationReviewStatus.New;
        vacancy.OwnerType = OwnerType.Provider;
        vacancy.VacancyReference = applicationReview.VacancyReference;
        vacancy.EmployerLocationOption = AvailableWhere.MultipleLocations;
        vacancy.EmployerLocations = ApiUtils.SerializeOrNull(addresses);

        providerUser.LastSignedInDate = DateTime.UtcNow.AddDays(-1);
        providerUser.UserType = UserType.Provider;
        providerUser.Ukprn = vacancy.Ukprn;
        providerUser.SetEmailPref(NotificationTypes.ApplicationSubmitted, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately);
        
        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet([applicationReview]);
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([providerUser]);
    
        // act
        var response = await Client.PostAsync($"{RouteNames.ApplicationReview}/{applicationReview.Id}/create-notifications", null);
        var notificationEmails = await response.Content.ReadAsAsync<List<NotificationEmail>>();
    
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notificationEmails.Should().HaveCount(1);
        notificationEmails[0].Tokens["location"].Should().Be(addresses.GetCityNames());
    }
    
    [Test, RecruitAutoData]
    public async Task Then_Daily_Notifications_Are_Stored_For_Later_Delivery(
        ApplicationReviewEntity applicationReview,
        VacancyEntity vacancy,
        UserEntity providerUser)
    {
        // arrange
        applicationReview.Status = ApplicationReviewStatus.New;
        vacancy.OwnerType = OwnerType.Provider;
        vacancy.VacancyReference = applicationReview.VacancyReference;
        vacancy.EmployerLocationOption = AvailableWhere.AcrossEngland;

        providerUser.UserType = UserType.Provider;
        providerUser.Ukprn = vacancy.Ukprn;
        providerUser.SetEmailPref(NotificationTypes.ApplicationSubmitted, NotificationScope.OrganisationVacancies, NotificationFrequency.Daily);
        var templateHelper = new EmailTemplateHelper(new DevelopmentEmailTemplateIds(), new DevelopmentRecruitBaseUrls("local"));
        
        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet([applicationReview]);
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([providerUser]);

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
        
        var notification = capturedNotifications?.SingleOrDefault();
        notification.Should().NotBeNull();
        notification.UserId.Should().Be(providerUser.Id);
        notification.EmailTemplateId.Should().Be(templateHelper.TemplateIds.ApplicationSubmittedToProviderDaily);
        notification.SendWhen.Should().BeCloseTo(DateTime.Now.GetNextDailySendDate(), TimeSpan.FromSeconds(5));
        
        var staticData = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(notification.StaticData)!;
        staticData["firstName"].Should().Be(providerUser.Name);
        staticData["notificationSettingsURL"].Should().Be(templateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn!.Value.ToString()));

        var dynamicData = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(notification.DynamicData)!;
        dynamicData["advertTitle"].Should().Be(vacancy.Title!);
        dynamicData["employerName"].Should().Be(vacancy.EmployerName);
        dynamicData["vacancyReference"].Should().Be(vacancy.VacancyReference.ToString());
        dynamicData["manageAdvertURL"].Should().Be($"{templateHelper.RecruitProviderBaseUrl}/{vacancy.Ukprn}/vacancies/{vacancy.Id}/manage");
        dynamicData["location"].Should().Be("Recruiting nationally");
    }
    
    [Test, RecruitAutoData]
    public async Task Then_Weekly_Notifications_Are_Stored_For_Later_Delivery(
        ApplicationReviewEntity applicationReview,
        VacancyEntity vacancy,
        UserEntity providerUser)
    {
        // arrange
        applicationReview.Status = ApplicationReviewStatus.New;
        vacancy.OwnerType = OwnerType.Provider;
        vacancy.VacancyReference = applicationReview.VacancyReference;
        vacancy.EmployerLocationOption = AvailableWhere.AcrossEngland;

        providerUser.UserType = UserType.Provider;
        providerUser.Ukprn = vacancy.Ukprn;
        providerUser.SetEmailPref(NotificationTypes.ApplicationSubmitted, NotificationScope.OrganisationVacancies, NotificationFrequency.Weekly);
        
        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet([applicationReview]);
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([providerUser]);
        
        var templateHelper = new EmailTemplateHelper(new DevelopmentEmailTemplateIds(), new DevelopmentRecruitBaseUrls("local"));

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
        
        var notification = capturedNotifications?.SingleOrDefault();
        notification.Should().NotBeNull();
        notification.UserId.Should().Be(providerUser.Id);
        notification.EmailTemplateId.Should().Be(templateHelper.TemplateIds.ApplicationSubmittedToProviderWeekly);
        notification.SendWhen.Should().BeCloseTo(DateTime.Now.GetNextWeeklySendDate(), TimeSpan.FromSeconds(5));

        var staticData = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(notification.StaticData)!;
        staticData["firstName"].Should().Be(providerUser.Name);
        staticData["notificationSettingsURL"].Should().Be(templateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn!.Value.ToString()));

        var dynamicData = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(notification.DynamicData)!;
        dynamicData["advertTitle"].Should().Be(vacancy.Title!);
        dynamicData["employerName"].Should().Be(vacancy.EmployerName);
        dynamicData["vacancyReference"].Should().Be(vacancy.VacancyReference.ToString());
        dynamicData["manageAdvertURL"].Should().Be($"{templateHelper.RecruitProviderBaseUrl}/{vacancy.Ukprn}/vacancies/{vacancy.Id}/manage");
        dynamicData["location"].Should().Be("Recruiting nationally");
    }
}