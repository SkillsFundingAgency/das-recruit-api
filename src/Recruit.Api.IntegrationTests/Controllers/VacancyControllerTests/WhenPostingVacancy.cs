using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Testing.Data;
using SFA.DAS.Recruit.Api.Testing.Http;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyControllerTests;

public class WhenPostingVacancy: BaseFixture
{
    [Test]
    public async Task Then_Without_Required_Fields_Bad_Request_Is_Returned()
    {
        // act
        var response = await Client.PostAsJsonAsync(RouteNames.Vacancies, new {});
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
        var vacancyReference = Fixture.Create<VacancyReference>();
        var request = Fixture.Create<PostVacancyRequest>();

        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(Fixture.CreateMany<UserEntity>());
        
        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(Fixture.CreateMany<VacancyEntity>(10).ToList());
        
        Server.DataContext
            .Setup(x => x.VacancyEntities.AddAsync(It.IsAny<VacancyEntity>(), It.IsAny<CancellationToken>()))
            .Callback((VacancyEntity entity, CancellationToken _) => { entity.Id = id; });
        
        Server.DataContext
            .Setup(x => x.GetNextVacancyReferenceAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancyReference.Value);
        
        // act
        var response = await Client.PostAsJsonAsync(RouteNames.Vacancies, request);
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        vacancy.Should().BeEquivalentTo(request, opt => opt
            .Excluding(x => x.SubmittedByUserId)
            .Excluding(x => x.ReviewRequestedByUserId));
        vacancy.Id.Should().Be(id);
        vacancy.VacancyReference.Should().Be(vacancyReference.Value);
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location.ToString().Should().Be($"/{RouteNames.Vacancies}/{vacancy.Id}");
        
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task Then_The_SubmittedUserId_Is_Looked_Up_IdamsUserId()
    {
        // arrange
        var id = Guid.NewGuid();
        var vacancyReference = Fixture.Create<VacancyReference>();
        var request = Fixture.Create<PostVacancyRequest>();
        var users = Fixture.CreateMany<UserEntity>(10).ToList();
        users[3].IdamsUserId = request.SubmittedByUserId;

        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(users);

        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(Fixture.CreateMany<VacancyEntity>(10).ToList());
        
        Server.DataContext
            .Setup(x => x.VacancyEntities.AddAsync(It.IsAny<VacancyEntity>(), It.IsAny<CancellationToken>()))
            .Callback((VacancyEntity entity, CancellationToken _) => { entity.Id = id; });
        
        Server.DataContext
            .Setup(x => x.GetNextVacancyReferenceAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancyReference.Value);
        
        // act
        var response = await Client.PostAsJsonAsync(RouteNames.Vacancies, request);
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        vacancy!.SubmittedByUserId.Should().Be(users[3].Id);
    }
    
    [Test]
    public async Task Then_The_SubmittedUserId_Is_Looked_Up_DfEUserId()
    {
        // arrange
        var id = Guid.NewGuid();
        var vacancyReference = Fixture.Create<VacancyReference>();
        var request = Fixture.Create<PostVacancyRequest>();
        var users = Fixture.CreateMany<UserEntity>(10).ToList();
        users[3].DfEUserId = request.SubmittedByUserId;

        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(users);

        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(Fixture.CreateMany<VacancyEntity>(10).ToList());
        
        Server.DataContext
            .Setup(x => x.VacancyEntities.AddAsync(It.IsAny<VacancyEntity>(), It.IsAny<CancellationToken>()))
            .Callback((VacancyEntity entity, CancellationToken _) => { entity.Id = id; });
        
        Server.DataContext
            .Setup(x => x.GetNextVacancyReferenceAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancyReference.Value);
        
        // act
        var response = await Client.PostAsJsonAsync(RouteNames.Vacancies, request);
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        vacancy!.SubmittedByUserId.Should().Be(users[3].Id);
    }
    
    [Test]
    public async Task Then_The_SubmittedUserId_Is_Looked_Up_UserId()
    {
        // arrange
        var id = Guid.NewGuid();
        var vacancyReference = Fixture.Create<VacancyReference>();
        var request = Fixture.Create<PostVacancyRequest>();
        var users = Fixture.CreateMany<UserEntity>(10).ToList();
        request.SubmittedByUserId = users[3].Id.ToString();

        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(users);

        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(Fixture.CreateMany<VacancyEntity>(10).ToList());
        
        Server.DataContext
            .Setup(x => x.VacancyEntities.AddAsync(It.IsAny<VacancyEntity>(), It.IsAny<CancellationToken>()))
            .Callback((VacancyEntity entity, CancellationToken _) => { entity.Id = id; });
        
        Server.DataContext
            .Setup(x => x.GetNextVacancyReferenceAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(vacancyReference.Value);
        
        // act
        var response = await Client.PostAsJsonAsync(RouteNames.Vacancies, request);
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        vacancy!.SubmittedByUserId.Should().Be(users[3].Id);
    }
}