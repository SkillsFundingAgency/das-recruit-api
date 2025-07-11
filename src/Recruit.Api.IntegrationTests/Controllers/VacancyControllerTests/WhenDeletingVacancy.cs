using System.Net;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyControllerTests;

public class WhenDeletingVacancy: BaseFixture
{
    private const string ProblemTitleText = "Cannot delete vacancy";
    private const string ProblemDetailText = "The vacancy is either already deleted or has a status which prevents it from being deleted";
    
    [Test]
    public async Task Then_The_Vacancy_Is_NotFound()
    {
        // arrange
        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(Fixture.CreateMany<VacancyEntity>(10).ToList());

        // act
        var response = await Client.DeleteAsync($"{RouteNames.Vacancies}/{Guid.NewGuid()}?UserId={Guid.NewGuid()}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [TestCase(VacancyStatus.Draft)]
    [TestCase(VacancyStatus.Referred)]
    [TestCase(VacancyStatus.Rejected)]
    public async Task Then_An_Open_Vacancy_With_A_Specific_State_Is_Soft_Deleted(VacancyStatus status)
    {
        // arrange - some gymnastics because the mock isn't stateful or a proper DbSet
        var items = Fixture.CreateMany<VacancyEntity>(10).ToList();
        var itemToDelete = items[5];
        itemToDelete.ClosingDate = DateTime.UtcNow.AddDays(1);
        itemToDelete.ClosedDate = null;
        itemToDelete.DeletedDate = null;
        itemToDelete.Status = status;
        
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet(items);
        var userId = Guid.NewGuid();

        // act
        var response = await Client.DeleteAsync($"{RouteNames.Vacancies}/{itemToDelete.Id}?UserId={userId}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        itemToDelete.DeletedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
    
    [Test]
    public async Task Then_A_Submitted_Vacancy_That_Has_Not_Closed_Can_Be_Deleted()
    {
        // arrange
        var vacancies = Fixture.CreateMany<VacancyEntity>(10).ToList();
        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(vacancies);
        
        var target = vacancies[4];
        target.Status = VacancyStatus.Submitted;
        target.ClosingDate = DateTime.UtcNow.AddDays(-1);
        target.ClosedDate = null;
        target.DeletedDate = null;

        // act
        var response = await Client.DeleteAsync($"{RouteNames.Vacancies}/{target.Id}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Test]
    public async Task Then_A_Submitted_Vacancy_That_Has_Closed_Cannot_Be_Deleted()
    {
        // arrange
        var vacancies = Fixture.CreateMany<VacancyEntity>(10).ToList();
        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(vacancies);
        
        var target = vacancies[4];
        target.Status = VacancyStatus.Submitted;
        target.ClosingDate = DateTime.UtcNow.AddHours(1);
        target.DeletedDate = null;

        // act
        var response = await Client.DeleteAsync($"{RouteNames.Vacancies}/{target.Id}");
        var problem = await response.Content.ReadAsAsync<ProblemDetails>();
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problem.Should().NotBeNull();
        problem.Title.Should().Be(ProblemTitleText);
        problem.Detail.Should().Be(ProblemDetailText);
    }
    
    [TestCase(VacancyStatus.Approved)]
    [TestCase(VacancyStatus.Live)]
    [TestCase(VacancyStatus.Review)]
    public async Task Then_An_Open_Vacancy_With_The_A_Specific_State_Cannot_Be_Deleted(VacancyStatus status)
    {
        // arrange
        var vacancies = Fixture.CreateMany<VacancyEntity>(10).ToList();
        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(vacancies);
        
        var target = vacancies[4];
        target.Status = status;
        target.ClosedDate = null;
        target.DeletedDate = null;

        // act
        var response = await Client.DeleteAsync($"{RouteNames.Vacancies}/{target.Id}");
        var problem = await response.Content.ReadAsAsync<ProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problem.Should().NotBeNull();
        problem.Title.Should().Be(ProblemTitleText);
        problem.Detail.Should().Be(ProblemDetailText);
    }
    
    [Test]
    public async Task Then_An_Closed_Vacancy_Cannot_Be_Deleted()
    {
        // arrange
        var vacancies = Fixture.CreateMany<VacancyEntity>(10).ToList();
        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(vacancies);
        
        var target = vacancies[4];
        target.Status = VacancyStatus.Closed;
        target.ClosureReason = ClosureReason.Auto;
        target.ClosedDate = DateTime.UtcNow.AddDays(-1);
        target.DeletedDate = null;

        // act
        var response = await Client.DeleteAsync($"{RouteNames.Vacancies}/{target.Id}");
        var problem = await response.Content.ReadAsAsync<ProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problem.Should().NotBeNull();
        problem.Title.Should().Be(ProblemTitleText);
        problem.Detail.Should().Be(ProblemDetailText);
    }
    
    [Test]
    public async Task Then_A_Deleted_Vacancy_Cannot_Be_Deleted()
    {
        // arrange
        var vacancies = Fixture.CreateMany<VacancyEntity>(10).ToList();
        Server.DataContext
            .Setup(x => x.VacancyEntities)
            .ReturnsDbSet(vacancies);
        
        var target = vacancies[4];
        target.DeletedDate = DateTime.UtcNow;

        // act
        var response = await Client.DeleteAsync($"{RouteNames.Vacancies}/{target.Id}");
        var problem = await response.Content.ReadAsAsync<ProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problem.Should().NotBeNull();
        problem.Title.Should().Be(ProblemTitleText);
        problem.Detail.Should().Be(ProblemDetailText);
    }
}