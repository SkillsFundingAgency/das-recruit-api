using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyReviewControllerTests;

public class WhenGettingVacancyReview: BaseFixture
{
    [Test]
    public async Task Then_The_VacancyReview_Is_Returned()
    {
        // arrange
        var items = Fixture.CreateMany<VacancyReviewEntity>(10).ToList();
        var expected = items[1];
        Server.DataContext.Setup(x => x.VacancyReviewEntities).ReturnsDbSet(items);

        // act
        var response = await Client.GetAsync($"{RouteNames.VacancyReviews}/{expected.Id}");
        var vacancyReview = await response.Content.ReadAsAsync<VacancyReview>();

        // assert
        response.EnsureSuccessStatusCode();
        vacancyReview.Should().BeEquivalentTo(expected, options => options
            .ExcludingMissingMembers()
            .Excluding(x => x.ManualQaFieldIndicators)
            .Excluding(x => x.UpdatedFieldIdentifiers)
            .Excluding(x => x.DismissedAutomatedQaOutcomeIndicators)
        );

        vacancyReview.ManualQaFieldIndicators.Should().BeEquivalentTo(JsonSerializer.Deserialize<List<string>>(expected.ManualQaFieldIndicators));
        vacancyReview.UpdatedFieldIdentifiers.Should().BeEquivalentTo(JsonSerializer.Deserialize<List<string>>(expected.UpdatedFieldIdentifiers));
        vacancyReview.DismissedAutomatedQaOutcomeIndicators.Should().BeEquivalentTo(JsonSerializer.Deserialize<List<string>>(expected.DismissedAutomatedQaOutcomeIndicators));
    }
    
    [Test]
    public async Task Then_The_VacancyReview_Is_NotFound()
    {
        // arrange
        Server.DataContext
            .Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(Fixture.CreateMany<VacancyReviewEntity>(10).ToList());

        // act
        var response = await Client.GetAsync($"{RouteNames.VacancyReviews}/{Guid.NewGuid()}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [TestCase("12341234")]
    [TestCase("VAC12341234")]
    public async Task Then_The_VacancyReviews_Are_Returned(string vacancyReference)
    {
        // arrange
        var items = Fixture.CreateMany<VacancyReviewEntity>(10).ToList();
        items[2].VacancyReference = vacancyReference;
        items[3].VacancyReference = vacancyReference;
        items[4].VacancyReference = vacancyReference;
        Server.DataContext.Setup(x => x.VacancyReviewEntities).ReturnsDbSet(items);
        
        var expected = items.Skip(2).Take(3);

        // act
        var response = await Client.GetAsync($"{RouteNames.Vacancies}/{vacancyReference}/reviews/");
        var vacancyReviews = await response.Content.ReadAsAsync<List<VacancyReview>>();

        // assert
        response.EnsureSuccessStatusCode();
        vacancyReviews.Should().HaveCount(3);
        vacancyReviews.Should().BeEquivalentTo(expected, options => options
            .ExcludingMissingMembers()
            .Excluding(x => x.ManualQaFieldIndicators)
            .Excluding(x => x.UpdatedFieldIdentifiers)
            .Excluding(x => x.DismissedAutomatedQaOutcomeIndicators)
        );
    }
    
    [TestCase("12341234")]
    [TestCase("VAC12341234")]
    public async Task Then_The_VacancyReviews_Are_NotFound(string vacancyReference)
    {
        // arrange
        Server.DataContext
            .Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(Fixture.Create<List<VacancyReviewEntity>>());

        // act
        var response = await Client.GetAsync($"{RouteNames.Vacancies}/{vacancyReference}/reviews/");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [TestCase("null")]
    [TestCase("foo")]
    [TestCase("VAC")]
    [TestCase("0")]
    [TestCase("-1")]
    public async Task Then_Providing_Invalid_VacancyReference_Returns_BadRequest(string vacancyReference)
    {
        // arrange
        Server.DataContext
            .Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(Fixture.Create<List<VacancyReviewEntity>>());

        // act
        var response = await Client.GetAsync($"{RouteNames.Vacancies}/{vacancyReference}/reviews/");
        var payload = await response.Content.ReadAsAsync<ValidationProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        payload.Should().NotBeNull();
        payload.Errors.Should().HaveCount(1);
        payload.Errors.Should().ContainKey("vacancyReference");
    }
}