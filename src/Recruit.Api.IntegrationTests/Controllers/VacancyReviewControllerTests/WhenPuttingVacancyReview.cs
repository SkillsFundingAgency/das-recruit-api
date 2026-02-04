using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.VacancyReview;
using SFA.DAS.Recruit.Api.UnitTests;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyReviewControllerTests;

public class WhenPuttingVacancyReview: BaseFixture
{
    [Test]
    public async Task Then_Without_Required_Fields_Bad_Request_Is_Returned()
    {
        // act
        var response = await Client.PutAsJsonAsync($"{RouteNames.VacancyReviews}/{Guid.NewGuid()}", new {});
        var errors = await response.Content.ReadAsAsync<ValidationProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Should().NotBeNull();
        errors.Errors.Should().HaveCount(3);
        errors.Errors.Should().ContainKeys(
            nameof(PutVacancyReviewRequest.VacancyReference),
            nameof(PutVacancyReviewRequest.VacancyTitle),
            nameof(PutVacancyReviewRequest.VacancySnapshot)
        );
    }
    
    [Test]
    public async Task Then_The_VacancyReview_Is_Added()
    {
        // arrange
        var id = Guid.NewGuid();
        Server.DataContext
            .Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(Fixture.CreateMany<VacancyReviewEntity>(10).ToList());

        var request = Fixture.Create<PutVacancyReviewRequest>();
        
        // act
        var response = await Client.PutAsJsonAsync($"{RouteNames.VacancyReviews}/{id}", request);
        var vacancyReview = await response.Content.ReadAsAsync<VacancyReview>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        vacancyReview.Should().BeEquivalentTo(request, opts => opts.Excluding(x => x.SubmittedByUserId));

        Server.DataContext.Verify(x => x.VacancyReviewEntities.AddAsync(ItIs.EquivalentTo(request.ToDomain(id)), It.IsAny<CancellationToken>()), Times.Once());
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task Then_The_VacancyReview_Is_Replaced()
    {
        // arrange
        var items = Fixture.CreateMany<VacancyReviewEntity>(10).ToList();
        var targetItem = items[5];
        Server.DataContext
            .Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(items);

        var request = Fixture.Create<PutVacancyReviewRequest>();
        
        // act
        var response = await Client.PutAsJsonAsync($"{RouteNames.VacancyReviews}/{targetItem.Id}", request);
        var vacancyReview = await response.Content.ReadAsAsync<VacancyReview>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        vacancyReview.Should().BeEquivalentTo(request, opts => opts.Excluding(x => x.SubmittedByUserId).Excluding(x=>x.SubmittedByUserEmail));
        vacancyReview.SubmittedByUserEmail.Should().Be(targetItem.SubmittedByUserEmail);
        var updatedItem = request.ToDomain(targetItem.Id);
        updatedItem.SubmittedByUserEmail = targetItem.SubmittedByUserEmail;
        Server.DataContext.Verify(x => x.SetValues(targetItem, ItIs.EquivalentTo(updatedItem)), Times.Once());
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Then_SubmittedByUserEmail_Is_Populated_When_Missing()
    {
        // arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = Fixture.Build<UserEntity>()
            .With(u => u.Id, userId)
            .With(u => u.Email, "test@user.test")
            .Create();
        Server.DataContext
            .Setup(x => x.VacancyReviewEntities)
            .ReturnsDbSet(new List<VacancyReviewEntity>());
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(new List<UserEntity> { user });

        var request = Fixture.Build<PutVacancyReviewRequest>()
            .Without(r => r.SubmittedByUserEmail)
            .With(r => r.SubmittedByUserId, userId.ToString())
            .Create();

        // act
        var response = await Client.PutAsJsonAsync($"{RouteNames.VacancyReviews}/{id}", request);
        var vacancyReview = await response.Content.ReadAsAsync<VacancyReview>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        vacancyReview!.SubmittedByUserEmail.Should().Be(user.Email);
        Server.DataContext.Verify(x => x.VacancyReviewEntities.AddAsync(It.Is<VacancyReviewEntity>(e => e.SubmittedByUserEmail == user.Email), It.IsAny<CancellationToken>()), Times.Once());
    }
}