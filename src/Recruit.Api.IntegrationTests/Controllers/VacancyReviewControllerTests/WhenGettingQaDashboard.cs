using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Requests.EmployerProfile;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyReviewControllerTests;
internal class WhenGettingQaDashboard : BaseFixture
{
    [Test]
    [MoqInlineAutoData(ReviewStatus.PendingReview)]
    [MoqInlineAutoData(ReviewStatus.UnderReview)]
    public async Task Then_The_QaDashboard_Model_Is_Returned(ReviewStatus status)
    {
        // arrange
        var items = Fixture.CreateMany<VacancyReviewEntity>(10).ToList();
        foreach (var vacancyReviewEntity in items)
        {
            vacancyReviewEntity.Status = status;
        }
        Server.DataContext.Setup(x => x.VacancyReviewEntities).ReturnsDbSet(items);

        // act
        var response = await Client.GetAsync($"{RouteNames.VacancyReviews}/qa/dashboard");
        var qaDashboard = await response.Content.ReadAsAsync<QaDashboard>();

        // assert
        response.EnsureSuccessStatusCode();
        qaDashboard.Should().NotBeNull();
        qaDashboard.TotalVacanciesForReview.Should().Be(items.Count);
    }

    [Test]
    [MoqInlineAutoData(ReviewStatus.New)]
    [MoqInlineAutoData(ReviewStatus.Closed)]
    public async Task Then_The_QaDashboard_Model_Is_Empty(ReviewStatus status)
    {
        // arrange
        var items = Fixture.CreateMany<VacancyReviewEntity>(10).ToList();
        foreach (var vacancyReviewEntity in items)
        {
            vacancyReviewEntity.Status = status;
        }
        Server.DataContext.Setup(x => x.VacancyReviewEntities).ReturnsDbSet(items);

        // act
        var response = await Client.GetAsync($"{RouteNames.VacancyReviews}/qa/dashboard");
        var qaDashboard = await response.Content.ReadAsAsync<QaDashboard>();

        // assert
        response.EnsureSuccessStatusCode();
        qaDashboard.Should().NotBeNull();
        qaDashboard.TotalVacanciesForReview.Should().Be(0);
        qaDashboard.TotalVacanciesBrokenSla.Should().Be(0);
        qaDashboard.TotalVacanciesResubmitted.Should().Be(0);
        qaDashboard.TotalVacanciesSubmittedTwelveTwentyFourHours.Should().Be(0);
    }

    [Test]
    public async Task Then_InternalServerException_Is_Returned()
    {
        // act
        var response = await Client.GetAsync($"{RouteNames.VacancyReviews}/qa/dashboard");
        var errors = await response.Content.ReadAsAsync<ProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        errors.Should().NotBeNull();
    }
}