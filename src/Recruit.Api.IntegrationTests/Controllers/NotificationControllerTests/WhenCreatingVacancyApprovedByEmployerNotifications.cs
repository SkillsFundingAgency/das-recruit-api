using System.Net;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.UnitTests;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.NotificationControllerTests;

public class WhenCreatingVacancyApprovedByEmployerNotifications: BaseFixture
{
    [Test, RecursiveMoqAutoData]
    public async Task And_No_Users_Are_Found_Then_No_Notifications_Are_Created(VacancyEntity vacancy)
    {
        // arrange
        vacancy.OwnerType = OwnerType.Provider;
        vacancy.Status = VacancyStatus.Submitted;
        
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
        string expectedHashedAccountId,
        long accountId,
        VacancyEntity vacancy,
        UserEntity providerUser)
    {
        // arrange
        vacancy.Status = VacancyStatus.Submitted;
        vacancy.AccountId = accountId;
        vacancy.OwnerType = OwnerType.Provider;
        vacancy.EmployerLocationOption = AvailableWhere.AcrossEngland;
        var templateHelper = new EmailTemplateHelper(new DevelopmentEmailTemplateIds(), new DevelopmentRecruitBaseUrls("local"));
        providerUser.UserType = UserType.Provider;
        providerUser.Ukprn = vacancy.Ukprn;
        providerUser.LastSignedInDate = DateTime.UtcNow.AddMinutes(-1);
        providerUser.SetEmailPref(NotificationTypes.VacancyApprovedOrRejected, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately);
        
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
            x.Tokens.Should().HaveCount(6);
            x.RecipientAddress.Should().Be(providerUser.Email);
            x.Tokens["firstName"].Should().Be(providerUser.Name);
            x.Tokens["advertTitle"].Should().Be(vacancy.Title!);
            x.Tokens["employerName"].Should().Be(vacancy.EmployerName);
            x.Tokens["VACcode"].Should().Be(vacancy.VacancyReference.ToString());
            x.Tokens["notificationSettingsURL"].Should().Be(templateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn!.Value.ToString()));
            x.Tokens["location"].Should().Be("Recruiting nationally");
        });
    }
}