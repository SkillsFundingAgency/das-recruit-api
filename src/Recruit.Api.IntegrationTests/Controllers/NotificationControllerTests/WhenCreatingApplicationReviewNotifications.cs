using System.Net;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.NotificationControllerTests;

public class WhenCreatingApplicationReviewNotifications : BaseFixture
{
    [Test]
    [MoqInlineAutoData(ApplicationReviewStatus.Successful)]
    [MoqInlineAutoData(ApplicationReviewStatus.Unsuccessful)]
    [MoqInlineAutoData(ApplicationReviewStatus.PendingShared)]
    [MoqInlineAutoData(ApplicationReviewStatus.PendingToMakeUnsuccessful)]
    [MoqInlineAutoData(ApplicationReviewStatus.InReview)]
    [MoqInlineAutoData(ApplicationReviewStatus.Interviewing)]
    [MoqInlineAutoData(ApplicationReviewStatus.AllShared)]
    public async Task And_No_Handler_Is_Registered_For_The_Status_Then_InternalServerError_Returned(ApplicationReviewStatus status, List<ApplicationReviewEntity> applicationReviews)
    {
        // arrange
        applicationReviews[1].Status = ApplicationReviewStatus.PendingShared;
        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet(applicationReviews);
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([]);

        // act
        var response = await Client.PostAsync($"{RouteNames.ApplicationReview}/{applicationReviews[1].Id}/create-notifications", null);
        var errors = await response.Content.ReadAsAsync<ProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        errors.Should().NotBeNull();
        errors.Title.Should().Be("Missing email handler");
    }
    
    [Test, MoqAutoData]
    public async Task And_ApplicationReview_Does_Not_Exist_Then_BadRequest_Returned(Guid id)
    {
        // arrange
        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet([]);

        // act
        var response = await Client.PostAsync($"{RouteNames.ApplicationReview}/{id}/create-notifications", null);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Test, MoqAutoData]
    public async Task And_Vacancy_Does_Not_Exist_Then_InternalServerError_Returned(List<ApplicationReviewEntity> applicationReviews)
    {
        // arrange
        applicationReviews[1].Status = ApplicationReviewStatus.Shared;
        Server.DataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet(applicationReviews);
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([]);

        // act
        var response = await Client.PostAsync($"{RouteNames.ApplicationReview}/{applicationReviews[1].Id}/create-notifications", null);
        var errors = await response.Content.ReadAsAsync<ProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        errors.Should().NotBeNull();
        errors.Title.Should().Be("Data integrity error");
    }
}