using System.Net;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Testing.Data;
using SFA.DAS.Recruit.Api.Testing.Http;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.NotificationControllerTests;

public class WhenCreatingVacancySentForReviewNotifications: BaseFixture
{
    [Test, RecursiveMoqAutoData]
    public async Task And_No_Users_Are_Found_Then_No_Notifications_Are_Created(VacancyEntity vacancy)
    {
        // arrange
        vacancy.Status = VacancyStatus.Review;
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
    public async Task Then_Immediate_Notifications_Are_Returned_For_Employer_User(
        string expectedHashedAccountId,
        long accountId,
        VacancyEntity vacancy,
        UserEntity employerUser)
    {
        // arrange
        vacancy.Status = VacancyStatus.Review;
        vacancy.AccountId = accountId;
        vacancy.OwnerType = OwnerType.Provider;
        vacancy.EmployerLocationOption = AvailableWhere.AcrossEngland;
        var templateHelper = new EmailTemplateHelper(new DevelopmentEmailTemplateIds(), new DevelopmentRecruitBaseUrls("local"));
        employerUser.UserType = UserType.Employer;
        employerUser.EmployerAccounts = [
            new UserEmployerAccountEntity {
                UserId = employerUser.Id,
                EmployerAccountId = accountId,
                User = employerUser
            }
        ];
        
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([employerUser]);
        Server.DataContext.Setup(x => x.UserEmployerAccountEntities).ReturnsDbSet(employerUser.EmployerAccounts);
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
            x.RecipientAddress.Should().Be(employerUser.Email);
            x.Tokens["firstName"].Should().Be(employerUser.Name);
            x.Tokens["advertTitle"].Should().Be(vacancy.Title!);
            x.Tokens["employerName"].Should().Be(vacancy.EmployerName);
            x.Tokens["vacancyReference"].Should().Be(vacancy.VacancyReference.ToString());
            x.Tokens["reviewAdvertURL"].Should().Be($"{templateHelper.RecruitEmployerBaseUrl}/accounts/{expectedHashedAccountId}/vacancies/{vacancy.Id}/check-answers");
            x.Tokens["location"].Should().Be("Recruiting nationally");
        });
    }
}