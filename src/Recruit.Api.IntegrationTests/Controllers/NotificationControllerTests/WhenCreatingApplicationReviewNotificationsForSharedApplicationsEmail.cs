using System.Net;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Encoding;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.NotificationControllerTests;

public class WhenCreatingApplicationReviewNotificationsForSharedApplicationsEmail : BaseFixture
{
    [Test, MoqAutoData]
    public async Task And_No_Users_Are_Found_Then_No_Notifications_Are_Created(
        List<ApplicationReviewEntity> applicationReviews,
        List<VacancyEntity> vacancies)
    {
        // arrange
        applicationReviews[1].Status = ApplicationReviewStatus.Shared;
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
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_Notifications_Are_Created(
        string expectedHashedAccountId,
        List<UserEntity> users,
        List<UserEmployerAccountEntity> userEmployerAccountEntities,
        ApplicationReviewEntity applicationReview,
        VacancyEntity vacancy)
    {
        // arrange
        applicationReview.Status = ApplicationReviewStatus.Shared;
        vacancy.VacancyReference = applicationReview.VacancyReference;
        var expectedUserNames = users.Select(x => x.Name).ToList();
        
        userEmployerAccountEntities.ForEach(x => x.EmployerAccountId = applicationReview.AccountId);
        for (int count = 0; count < users.Count; count++)
        {
            var userAccount = userEmployerAccountEntities[count];
            var user = users[count];
            userAccount.User = user;
            userAccount.UserId = user.Id;
            userAccount.EmployerAccountId = applicationReview.AccountId;
        }

        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet([applicationReview]);
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        Server.DataContext.Setup(x => x.UserEntities).ReturnsDbSet(users);
        Server.DataContext.Setup(x => x.UserEmployerAccountEntities).ReturnsDbSet(userEmployerAccountEntities);

        Server.EncodingService
            .Setup(x => x.Encode(applicationReview.AccountId, EncodingType.AccountId))
            .Returns(expectedHashedAccountId);

        // act
        var response = await Client.PostAsync($"{RouteNames.ApplicationReview}/{applicationReview.Id}/create-notifications", null);
        var notificationEmails = await response.Content.ReadAsAsync<List<NotificationEmail>>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notificationEmails.Should().HaveCount(users.Count);
        notificationEmails.Should().AllSatisfy(x =>
        {
            x.Tokens.Should().HaveCount(5);
            expectedUserNames.Should().Contain(x.Tokens["firstName"]);
            x.Tokens["trainingProvider"].Should().Be(vacancy.TrainingProvider_Name!);
            x.Tokens["advertTitle"].Should().Be(vacancy.Title!);
            x.Tokens["vacancyReference"].Should().Be(vacancy.VacancyReference.ToString()!);
            x.Tokens["applicationUrl"].Should()
                .EndWith($"/accounts/{expectedHashedAccountId}/vacancies/{vacancy.Id}/applications/{applicationReview.ApplicationId}/?vacancySharedByProvider=True");
        });
    }
}