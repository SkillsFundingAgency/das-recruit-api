using System.Net;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Testing.Data;
using SFA.DAS.Recruit.Api.Testing.Http;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.NotificationControllerTests;

public class WhenCreatingVacancyRejectedByEmployerNotifications: BaseFixture
{
    [Test, RecursiveMoqAutoData]
    public async Task And_No_Users_Are_Found_Then_No_Notifications_Are_Created(VacancyEntity vacancy)
    {
        // arrange
        vacancy.OwnerType = OwnerType.Provider;
        vacancy.Status = VacancyStatus.Rejected;
        vacancy.EmployerRejectedReason = "Some reason";
        
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
        vacancy.Status = VacancyStatus.Rejected;
        vacancy.AccountId = accountId;
        vacancy.OwnerType = OwnerType.Provider;
        vacancy.EmployerLocationOption = AvailableWhere.AcrossEngland;
        vacancy.EmployerRejectedReason = "Some reason";

        providerUser.LastSignedInDate = DateTime.UtcNow;
        providerUser.UserType = UserType.Provider;
        providerUser.Ukprn = vacancy.Ukprn;
        providerUser.SetEmailPref(NotificationTypes.VacancyApprovedOrRejected, NotificationScope.OrganisationVacancies, NotificationFrequency.Immediately);
        
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([providerUser]);
        Server.DataContext.Setup(x => x.UserEmployerAccountEntities).ReturnsDbSet([]);
    
        var templateHelper = new EmailTemplateHelper(new DevelopmentEmailTemplateIds(), new DevelopmentRecruitBaseUrls("local"));
        
        // act
        var response = await Client.PostAsync($"{RouteNames.Vacancies}/{vacancy.Id}/create-notifications", null);
        var notificationEmails = await response.Content.ReadAsAsync<List<NotificationEmail>>();
    
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notificationEmails.Should().HaveCount(1);
        notificationEmails.Should().AllSatisfy(x =>
        {
            x.RecipientAddress.Should().Be(providerUser.Email);
            x.TemplateId.Should().Be(templateHelper.TemplateIds.ProviderVacancyRejectedByEmployer);
            x.Tokens.Should().HaveCount(7);
            x.Tokens["firstName"].Should().Be(providerUser.Name);
            x.Tokens["vacancyTitle"].Should().Be(vacancy.Title!);
            x.Tokens["employerName"].Should().Be(vacancy.EmployerName);
            x.Tokens["vacancyReference"].Should().Be(vacancy.VacancyReference.ToString());
            x.Tokens["notificationSettingsURL"].Should().Be(templateHelper.ProviderManageNotificationsUrl(vacancy.Ukprn!.Value.ToString()));
            x.Tokens["rejectedEmployerVacancyURL"].Should().Be($"{templateHelper.RecruitProviderBaseUrl}/{vacancy.Ukprn}/vacancies/{vacancy.Id}/check-your-answers");
            x.Tokens["location"].Should().Be("Recruiting nationally");
        });
    }
}