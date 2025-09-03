using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.UnitTests;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyControllerTests;

public class WhenPuttingVacancy: BaseFixture
{
    [Test]
    public async Task Then_Without_Required_Fields_Bad_Request_Is_Returned()
    {
        // act
        var response = await Client.PutAsJsonAsync($"{RouteNames.Vacancies}/{Guid.NewGuid()}", new {});
        var errors = await response.Content.ReadAsAsync<ValidationProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Should().NotBeNull();
        errors.Errors.Should().HaveCount(2);
        errors.Errors.Should().ContainKeys(
            nameof(PutVacancyRequest.OwnerType),
            nameof(PutVacancyRequest.Status)
        );
    }
    
    [Test]
    public async Task Then_The_Vacancy_Is_Added()
    {
        // arrange
        var id = Guid.NewGuid();
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(Fixture.CreateMany<UserEntity>(10));
        
        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(Fixture.CreateMany<VacancyEntity>(10).ToList());

        var request = Fixture.Create<PutVacancyRequest>();
        var expectedEntity = request.ToDomain(id);
        
        // act
        var response = await Client.PutAsJsonAsync($"{RouteNames.Vacancies}/{id}", request);
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        vacancy.Should().BeEquivalentTo(request, opt => opt
            .Excluding(x => x.SubmittedByUserId)
            .Excluding(x => x.ReviewRequestedByUserId)
        );
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location.ToString().Should().Be($"/{RouteNames.Vacancies}/{vacancy.Id}");

        Server.DataContext.Verify(x => x.VacancyEntities.AddAsync(ItIs.EquivalentTo(expectedEntity), It.IsAny<CancellationToken>()), Times.Once());
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task Then_The_Vacancy_Is_Replaced()
    {
        // arrange
        var items = Fixture.CreateMany<VacancyEntity>(10).ToList();
        var targetItem = items[5];
        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(items);
        
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(Fixture.CreateMany<UserEntity>(10));

        var request = Fixture.Create<PutVacancyRequest>();
        
        // act
        var response = await Client.PutAsJsonAsync($"{RouteNames.Vacancies}/{targetItem.Id}", request);
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        vacancy.Should().BeEquivalentTo(request, opt => opt
            .Excluding(x => x.SubmittedByUserId)
            .Excluding(x => x.ReviewRequestedByUserId));

        Server.DataContext.Verify(x => x.SetValues(targetItem, ItIs.EquivalentTo(request.ToDomain(targetItem.Id))), Times.Once());
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}