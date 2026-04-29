using System.Net;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Core.Email;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.NotificationControllerTests;

public class WhenCreatingVacancyClosedByQaNotifications: BaseFixture
{
    [Test]
    [RecursiveMoqInlineAutoData(ClosureReason.Auto)]
    [RecursiveMoqInlineAutoData(ClosureReason.BlockedByQa)]
    [RecursiveMoqInlineAutoData(ClosureReason.Manual)]
    [RecursiveMoqInlineAutoData(ClosureReason.TransferredByEmployer)]
    [RecursiveMoqInlineAutoData(ClosureReason.TransferredByQa)]
    public async Task And_The_Vacancy_Was_Not_Closed_By_Qa_Then_No_Notifications_Are_Created_For_Provider_Users(ClosureReason closureReason, VacancyEntity vacancy, UserEntity providerUser)
    {
        // arrange
        vacancy.OwnerType = OwnerType.Provider;
        vacancy.Status = VacancyStatus.Closed;
        vacancy.ClosureReason = closureReason;
        
        providerUser.UserType = UserType.Provider;
        providerUser.Ukprn = vacancy.Ukprn;
        providerUser.LastSignedInDate = DateTime.UtcNow.AddMinutes(-1);

        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([providerUser]);
        Server.DataContext.Setup(x => x.UserEmployerAccountEntities).ReturnsDbSet([]);

        // act
        var response = await Client.PostAsync($"{RouteNames.Vacancies}/{vacancy.Id}/create-notifications", null);
        var notificationEmails = await response.Content.ReadAsAsync<List<NotificationEmail>>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notificationEmails.Should().HaveCount(0);
    }
    
    [Test]
    [RecursiveMoqInlineAutoData(ClosureReason.Auto)]
    [RecursiveMoqInlineAutoData(ClosureReason.BlockedByQa)]
    [RecursiveMoqInlineAutoData(ClosureReason.Manual)]
    [RecursiveMoqInlineAutoData(ClosureReason.TransferredByEmployer)]
    [RecursiveMoqInlineAutoData(ClosureReason.TransferredByQa)]
    public async Task And_The_Vacancy_Was_Not_Closed_By_Qa_Then_No_Notifications_Are_Created_For_Employer_Users(
        ClosureReason closureReason,
        VacancyEntity vacancy,
        UserEntity employerUser,
        UserEmployerAccountEntity employerAccount)
    {
        // arrange
        vacancy.OwnerType = OwnerType.Employer;
        vacancy.Status = VacancyStatus.Closed;
        vacancy.ClosureReason = closureReason;
        
        employerUser.UserType = UserType.Employer;
        employerUser.LastSignedInDate = DateTime.UtcNow.AddMinutes(-1);
        employerUser.EmployerAccounts = [employerAccount];
        employerAccount.User = employerUser;
        employerAccount.UserId = employerUser.Id;
        employerAccount.EmployerAccountId = vacancy.AccountId!.Value;

        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet([employerUser]);
        Server.DataContext.Setup(x => x.UserEmployerAccountEntities).ReturnsDbSet([employerAccount]);

        // act
        var response = await Client.PostAsync($"{RouteNames.Vacancies}/{vacancy.Id}/create-notifications", null);
        var notificationEmails = await response.Content.ReadAsAsync<List<NotificationEmail>>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notificationEmails.Should().HaveCount(0);
    }
    
    [Test]
    [RecursiveMoqInlineAutoData(OwnerType.Employer)]
    [RecursiveMoqInlineAutoData(OwnerType.Provider)]
    public async Task And_No_Users_Are_Found_Then_No_Notifications_Are_Created(OwnerType ownerType, VacancyEntity vacancy)
    {
        // arrange
        vacancy.OwnerType = ownerType;
        vacancy.Status = VacancyStatus.Closed;
        vacancy.ClosureReason = ClosureReason.WithdrawnByQa;
        
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
        vacancy.Status = VacancyStatus.Closed;
        vacancy.ClosureReason = ClosureReason.WithdrawnByQa;
        vacancy.AccountId = accountId;
        vacancy.OwnerType = OwnerType.Provider;
        var templateHelper = new EmailTemplateHelper(new DevelopmentEmailTemplateIds(), new DevelopmentRecruitBaseUrls("local"), new DevelopmentFaaBaseUrls("local"));
        providerUser.UserType = UserType.Provider;
        providerUser.Ukprn = vacancy.Ukprn;
        providerUser.LastSignedInDate = DateTime.UtcNow.AddMinutes(-1);
        
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
            x.Tokens.Should().HaveCount(3);
            x.TemplateId.Should().Be(templateHelper.TemplateIds.VacancyWithdrawnByQa);
            x.RecipientAddress.Should().Be(providerUser.Email);
            x.Tokens["user-name"].Should().Be(providerUser.Name);
            x.Tokens["vacancy-title"].Should().Be(vacancy.Title!);
            x.Tokens["vacancy-reference"].Should().Be(vacancy.VacancyReference.ToString());
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
        vacancy.Status = VacancyStatus.Closed;
        vacancy.ClosureReason = ClosureReason.WithdrawnByQa;
        vacancy.AccountId = accountId;
        vacancy.OwnerType = OwnerType.Employer;
        var templateHelper = new EmailTemplateHelper(new DevelopmentEmailTemplateIds(), new DevelopmentRecruitBaseUrls("local"), new DevelopmentFaaBaseUrls("local"));
        employerUser.UserType = UserType.Employer;
        employerUser.LastSignedInDate = DateTime.UtcNow.AddMinutes(-1);
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
            x.Tokens.Should().HaveCount(3);
            x.TemplateId.Should().Be(templateHelper.TemplateIds.VacancyWithdrawnByQa);
            x.RecipientAddress.Should().Be(employerUser.Email);
            x.Tokens["user-name"].Should().Be(employerUser.Name);
            x.Tokens["vacancy-title"].Should().Be(vacancy.Title!);
            x.Tokens["vacancy-reference"].Should().Be(vacancy.VacancyReference.ToString());
        });
    }
}