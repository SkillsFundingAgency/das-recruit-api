using System.Net;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyControllerTests;

public class WhenDeletingVacancy: MsSqlBaseFixture
{
    private const string ProblemTitleText = "Cannot delete vacancy";
    private const string ProblemDetailText = "The vacancy is either already deleted or has a status which prevents it from being deleted";
    
    [Test]
    public async Task Then_The_Vacancy_Is_NotFound()
    {
        // act
        var response = await Measure.ThisAsync(async () => await Client.DeleteAsync($"{RouteNames.Vacancies}/{Guid.NewGuid()}"));

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [TestCase(VacancyStatus.Draft)]
    [TestCase(VacancyStatus.Referred)]
    [TestCase(VacancyStatus.Rejected)]
    public async Task Then_An_Open_Vacancy_With_A_Specific_State_Is_Soft_Deleted(VacancyStatus status)
    {
        // arrange
        var vacancy = await TestData.Create<VacancyEntity>(x =>
        {
            x.ClosingDate = DateTime.UtcNow.AddDays(1);
            x.ClosedDate = null;
            x.DeletedDate = null;
            x.Status = status;
        });
        
        // act
        var response = await Measure.ThisAsync(async () => await Client.DeleteAsync($"{RouteNames.Vacancies}/{vacancy.Id}"));

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var record = await TestData.Get<VacancyEntity>(vacancy.Id);
        record!.DeletedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
    
    [Test]
    public async Task Then_A_Submitted_Vacancy_That_Has_Not_Closed_Can_Be_Deleted()
    {
        // arrange
        var vacancy = await TestData.Create<VacancyEntity>(x =>
        {
            x.Status = VacancyStatus.Submitted;
            x.ClosingDate = DateTime.UtcNow.AddDays(-1);
            x.ClosedDate = null;
            x.DeletedDate = null;
        });
        
        // act
        var response = await Measure.ThisAsync(async () => await Client.DeleteAsync($"{RouteNames.Vacancies}/{vacancy.Id}"));

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Test]
    public async Task Then_A_Submitted_Vacancy_That_Has_Closed_Cannot_Be_Deleted()
    {
        // arrange
        var vacancy = await TestData.Create<VacancyEntity>(x =>
        {
            x.Status = VacancyStatus.Submitted;
            x.ClosingDate = DateTime.UtcNow.AddHours(1);
            x.DeletedDate = null;
        });

        // act
        var response = await Measure.ThisAsync(async () => await Client.DeleteAsync($"{RouteNames.Vacancies}/{vacancy.Id}"));
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
    public async Task Then_An_Open_Vacancy_With_The_Specific_State_Cannot_Be_Deleted(VacancyStatus status)
    {
        // arrange
        var vacancy = await TestData.Create<VacancyEntity>(x =>
        {
            x.Status = status;
            x.ClosedDate = null;
            x.DeletedDate = null;
        });
        
        // act
        var response = await Measure.ThisAsync(async () => await Client.DeleteAsync($"{RouteNames.Vacancies}/{vacancy.Id}"));
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
        var vacancy = await TestData.Create<VacancyEntity>(x =>
        {
            x.Status = VacancyStatus.Closed;
            x.ClosureReason = ClosureReason.Auto;
            x.ClosedDate = DateTime.UtcNow.AddDays(-1);
            x.DeletedDate = null;
        });
        
        // act
        var response = await Measure.ThisAsync(async () => await Client.DeleteAsync($"{RouteNames.Vacancies}/{vacancy.Id}"));
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
        var vacancy = await TestData.Create<VacancyEntity>(x =>
        {
            x.DeletedDate = DateTime.UtcNow;
        });
        
        // act
        var response = await Measure.ThisAsync(async () => await Client.DeleteAsync($"{RouteNames.Vacancies}/{vacancy.Id}"));
        var problem = await response.Content.ReadAsAsync<ProblemDetails>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problem.Should().NotBeNull();
        problem.Title.Should().Be(ProblemTitleText);
        problem.Detail.Should().Be(ProblemDetailText);
    }
}