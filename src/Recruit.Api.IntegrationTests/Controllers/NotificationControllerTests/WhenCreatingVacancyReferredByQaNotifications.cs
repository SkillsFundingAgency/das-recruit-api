using System.Net;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.UnitTests;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.NotificationControllerTests;

public class WhenCreatingVacancyReferredByQaNotifications: BaseFixture
{
    [Test]
    [RecursiveMoqInlineAutoData(OwnerType.Employer)]
    [RecursiveMoqInlineAutoData(OwnerType.Provider)]
    public async Task And_No_Users_Are_Found_Then_No_Notifications_Are_Created(OwnerType ownerType, VacancyEntity vacancy)
    {
        // arrange
        vacancy.OwnerType = ownerType;
        vacancy.Status = VacancyStatus.Referred;
        
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
    public async Task Then_Immediate_Notifications_Are_Returned_For_Provider_User(
        long accountId,
        VacancyEntity vacancy,
        UserEntity providerUser)
    {
        // arrange
        vacancy.Status = VacancyStatus.Referred;
        vacancy.AccountId = accountId;
        vacancy.OwnerType = OwnerType.Provider;
        vacancy.EmployerLocationOption = AvailableWhere.AcrossEngland;
        var templateHelper = new EmailTemplateHelper(new DevelopmentEmailTemplateIds(), new DevelopmentRecruitBaseUrls("local"), new DevelopmentFaaBaseUrls("local"));
        providerUser.UserType = UserType.Provider;
        providerUser.Ukprn = vacancy.Ukprn;
        providerUser.LastSignedInDate = DateTime.UtcNow.AddMinutes(-1);
        providerUser.InitEmailPref(NotificationTypes.VacancyApprovedOrRejected, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately);
        
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
            x.TemplateId.Should().Be(templateHelper.TemplateIds.ProviderVacancyRejectedByDfe);
            x.RecipientAddress.Should().Be(providerUser.Email);
            x.Tokens["firstName"].Should().Be(providerUser.Name);
            x.Tokens["advertTitle"].Should().Be(vacancy.Title!);
            x.Tokens["employerName"].Should().Be(vacancy.EmployerName);
            x.Tokens["VACcode"].Should().Be(vacancy.VacancyReference.ToString());
            x.Tokens["rejectedAdvertURL"].Should().Be(templateHelper.ProviderReviewVacancyUrl(vacancy.Ukprn!.Value, vacancy.Id));
            x.Tokens["notificationSettingsURL"].Should().Be(templateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn!.Value.ToString()));
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
        vacancy.Status = VacancyStatus.Referred;
        vacancy.AccountId = accountId;
        vacancy.OwnerType = OwnerType.Employer;
        vacancy.EmployerLocationOption = AvailableWhere.AcrossEngland;
        var templateHelper = new EmailTemplateHelper(new DevelopmentEmailTemplateIds(), new DevelopmentRecruitBaseUrls("local"), new DevelopmentFaaBaseUrls("local"));
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
            x.TemplateId.Should().Be(templateHelper.TemplateIds.EmployerVacancyRejectedByDfe);
            x.RecipientAddress.Should().Be(employerUser.Email);
            x.Tokens["firstName"].Should().Be(employerUser.Name);
            x.Tokens["advertTitle"].Should().Be(vacancy.Title!);
            x.Tokens["employerName"].Should().Be(vacancy.EmployerName);
            x.Tokens["VACcode"].Should().Be(vacancy.VacancyReference.ToString());
            x.Tokens["rejectedAdvertURL"].Should().Be(templateHelper.EmployerReviewVacancyUrl(expectedHashedAccountId, vacancy.Id));
            x.Tokens["notificationSettingsURL"].Should().Be(templateHelper.EmployerManageNotificationsUrl(expectedHashedAccountId));
            x.Tokens["location"].Should().Be("Recruiting nationally");
        });
    }
}