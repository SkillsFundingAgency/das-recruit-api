using System.Net;
using System.Net.Http.Json;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.UnitTests;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.NotificationControllerTests;

public class WhenCreatingVacancyApprovedByQaNotifications: BaseFixture
{
    private readonly EmailTemplateHelper _templateHelper = new (new DevelopmentEmailTemplateIds(), new DevelopmentRecruitBaseUrls("local"), new DevelopmentFaaBaseUrls("local"));

    private static void ConvertToProviderUser(UserEntity user, VacancyEntity vacancy)
    {
        user.UserType = UserType.Provider;
        user.Ukprn = vacancy.Ukprn;
        user.LastSignedInDate = DateTime.UtcNow.AddMinutes(-1);
    }

    private static void InitVacancy(VacancyEntity vacancy, OwnerType ownerType, long? accountId = null)
    {
        vacancy.AccountId = accountId ?? vacancy.AccountId;
        vacancy.OwnerType = ownerType;
        vacancy.Status = VacancyStatus.Approved;
        vacancy.EmployerLocationOption = AvailableWhere.AcrossEngland;
        vacancy.NumberOfPositions = 5;
        vacancy.StartDate = new DateTime(2026, 6, 14);
        vacancy.Wage_DurationUnit = DurationUnit.Week;
        vacancy.Wage_Duration = 4;
    }
    

    [Test]
    [RecursiveMoqInlineAutoData(OwnerType.Employer)]
    [RecursiveMoqInlineAutoData(OwnerType.Provider)]
    public async Task And_No_Users_Are_Found_Then_No_Notifications_Are_Created(OwnerType ownerType, VacancyEntity vacancy)
    {
        // arrange
        vacancy.OwnerType = ownerType;
        vacancy.Status = VacancyStatus.Approved;
        
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([]);
        Server.DataContext.Setup(x => x.UserEmployerAccountEntities).ReturnsDbSet([]);

        // act
        var response = await Client.PostAsync($"{RouteNames.Vacancies}/{vacancy.Id}/create-notifications", null);
        var notificationEmails = await response.Content.ReadAsAsync<List<NotificationEmail>>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notificationEmails.Should().HaveCount(0);
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_Immediate_Approval_Notifications_Are_Returned_For_Provider_User(
        VacancyEntity vacancy,
        UserEntity providerUser)
    {
        // arrange
        InitVacancy(vacancy, OwnerType.Provider);
        ConvertToProviderUser(providerUser, vacancy);
        providerUser
            .InitEmailPref(NotificationTypes.VacancyApprovedOrRejected, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately)
            .SetEmailPref(NotificationTypes.ProviderAttachedToVacancy, NotificationScope.NotSet, NotificationFrequency.Never);
        
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([providerUser]);
        Server.DataContext.Setup(x => x.UserEmployerAccountEntities).ReturnsDbSet([]);
    
        // act
        var response = await Client.PostAsync($"{RouteNames.Vacancies}/{vacancy.Id}/create-notifications", null);
        var notificationEmails = await response.Content.ReadAsAsync<List<NotificationEmail>>();
    
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notificationEmails.Should().HaveCount(1);
        notificationEmails.Should().AllSatisfy(x =>
        {
            x.Tokens.Should().HaveCount(7);
            x.TemplateId.Should().Be(_templateHelper.TemplateIds.ProviderVacancyApprovedByDfe);
            x.RecipientAddress.Should().Be(providerUser.Email);
            x.Tokens["firstName"].Should().Be(providerUser.Name);
            x.Tokens["advertTitle"].Should().Be(vacancy.Title!);
            x.Tokens["employerName"].Should().Be(vacancy.EmployerName);
            x.Tokens["VACcode"].Should().Be(vacancy.VacancyReference.ToString());
            x.Tokens["FindAnApprenticeshipAdvertURL"].Should().Be(_templateHelper.FaaVacancyUrl(vacancy.VacancyReference));
            x.Tokens["notificationSettingsURL"].Should().Be(_templateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn!.Value.ToString()));
            x.Tokens["location"].Should().Be("Recruiting nationally");
        });
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_Immediate_Notifications_Are_Returned_For_Employer_User(
        string expectedHashedAccountId,
        long accountId,
        VacancyEntity vacancy,
        UserEntity employerUser,
        UserEmployerAccountEntity employerAccount)
    {
        // arrange
        InitVacancy(vacancy, OwnerType.Employer, accountId);
        employerUser.UserType = UserType.Employer;
        employerUser.LastSignedInDate = DateTime.UtcNow.AddMinutes(-1);
        employerUser.InitEmailPref(NotificationTypes.VacancyApprovedOrRejected, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately);
        employerUser.EmployerAccounts = [employerAccount];
        employerAccount.User = employerUser;
        employerAccount.UserId = employerUser.Id;
        employerAccount.EmployerAccountId = accountId;
        
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([employerUser]);
        Server.DataContext.Setup(x => x.UserEmployerAccountEntities).ReturnsDbSet([employerAccount]);
        Server.EncodingService
            .Setup(x => x.Encode(accountId, EncodingType.AccountId))
            .Returns(expectedHashedAccountId);
    
        // act
        var response = await Client.PostAsync($"{RouteNames.Vacancies}/{vacancy.Id}/create-notifications", null);
        var notificationEmails = await response.Content.ReadAsAsync<List<NotificationEmail>>();
    
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notificationEmails.Should().HaveCount(1);
        notificationEmails.Should().AllSatisfy(x =>
        {
            x.Tokens.Should().HaveCount(7);
            x.TemplateId.Should().Be(_templateHelper.TemplateIds.EmployerVacancyApprovedByDfe);
            x.RecipientAddress.Should().Be(employerUser.Email);
            x.Tokens["firstName"].Should().Be(employerUser.Name);
            x.Tokens["advertTitle"].Should().Be(vacancy.Title!);
            x.Tokens["employerName"].Should().Be(vacancy.EmployerName);
            x.Tokens["VACcode"].Should().Be(vacancy.VacancyReference.ToString());
            x.Tokens["FindAnApprenticeshipAdvertURL"].Should().Be(_templateHelper.FaaVacancyUrl(vacancy.VacancyReference));
            x.Tokens["notificationSettingsURL"].Should().Be(_templateHelper.EmployerManageNotificationsUrl(expectedHashedAccountId));
            x.Tokens["location"].Should().Be("Recruiting nationally");
        });
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_Immediate_Attached_Notifications_Are_Returned_For_Provider_User(
        VacancyEntity vacancy,
        UserEntity providerUser,
        string courseTitle)
    {
        // arrange
        InitVacancy(vacancy, OwnerType.Employer);
        ConvertToProviderUser(providerUser, vacancy);
        providerUser.InitEmailPref(NotificationTypes.ProviderAttachedToVacancy, NotificationScope.NotSet, NotificationFrequency.Immediately);
        
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([providerUser]);
        Server.DataContext.Setup(x => x.UserEmployerAccountEntities).ReturnsDbSet([]);

        var httpContent = JsonContent.Create(new Dictionary<string, string> { ["courseTitle"] = courseTitle });
    
        // act
        var response = await Client.PostAsync($"{RouteNames.Vacancies}/{vacancy.Id}/create-notifications", httpContent);
        var notificationEmails = await response.Content.ReadAsAsync<List<NotificationEmail>>();
    
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notificationEmails.Should().HaveCount(1);
        notificationEmails.Should().AllSatisfy(x =>
        {
            x.Tokens.Should().HaveCount(12);
            x.TemplateId.Should().Be(_templateHelper.TemplateIds.ProviderAttachedToVacancy);
            x.RecipientAddress.Should().Be(providerUser.Email);
            x.Tokens["firstName"].Should().Be(providerUser.Name);
            x.Tokens["advertTitle"].Should().Be(vacancy.Title!);
            x.Tokens["employer"].Should().Be(vacancy.EmployerName);
            x.Tokens["VACnumber"].Should().Be(vacancy.VacancyReference.ToString());
            x.Tokens["applicationURL"].Should().Be(_templateHelper.FaaVacancyUrl(vacancy.VacancyReference));
            x.Tokens["notificationSettingsURL"].Should().Be(_templateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn!.Value.ToString()));
            x.Tokens["location"].Should().Be("Recruiting nationally");
            x.Tokens["positions"].Should().Be("5 apprentices");
            x.Tokens["startDate"].Should().Be("14 June 2026");
            x.Tokens["duration"].Should().Be("4 weeks");
            x.Tokens["submitterEmail"].Should().Be("Contact details not found");
            x.Tokens["courseTitle"].Should().Be(courseTitle);
        });
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Submitting_User_Is_Looked_Up_For_The_Provider_Attached_Email(
        VacancyEntity vacancy,
        UserEntity providerUser,
        string courseTitle,
        long accountId,
        UserEntity employerUser,
        UserEmployerAccountEntity employerAccount)
    {
        // arrange
        InitVacancy(vacancy, OwnerType.Employer, accountId);
        ConvertToProviderUser(providerUser, vacancy);
        vacancy.SubmittedByUserId = employerUser.Id;
        
        employerUser.UserType = UserType.Employer;
        employerUser.LastSignedInDate = DateTime.UtcNow.AddMinutes(-1);
        employerUser.InitEmailPref(NotificationTypes.VacancyApprovedOrRejected, NotificationScope.OrganisationVacancies, NotificationFrequency.Never);
        employerUser.EmployerAccounts = [employerAccount];
        employerAccount.User = employerUser;
        employerAccount.UserId = employerUser.Id;
        employerAccount.EmployerAccountId = accountId;
        providerUser.InitEmailPref(NotificationTypes.ProviderAttachedToVacancy, NotificationScope.NotSet, NotificationFrequency.Immediately);
        
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([providerUser, employerUser]);
        Server.DataContext.Setup(x => x.UserEmployerAccountEntities).ReturnsDbSet([employerAccount]);

        // act
        var response = await Client.PostAsync($"{RouteNames.Vacancies}/{vacancy.Id}/create-notifications", null);
        var notificationEmails = await response.Content.ReadAsAsync<List<NotificationEmail>>();
    
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notificationEmails.Should().HaveCount(1);
        notificationEmails.Should().AllSatisfy(x =>
        {
            x.Tokens["submitterEmail"].Should().Be(employerUser.Email);
        });
    }
}