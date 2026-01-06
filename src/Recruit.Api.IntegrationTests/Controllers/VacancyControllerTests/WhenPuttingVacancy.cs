using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Testing;
using SFA.DAS.Recruit.Api.Testing.Diagnostics;
using SFA.DAS.Recruit.Api.Testing.Http;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.VacancyControllerTests;

public class WhenPuttingVacancy: MsSqlBaseFixture
{
    [Test]
    public async Task Then_Without_Required_Fields_Bad_Request_Is_Returned()
    {
        // act
        var response = await Measure.ThisAsync(async () => await Client.PutAsJsonAsync($"{RouteNames.Vacancies}/{Guid.NewGuid()}", new {}));
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
        var request = Fixture.Create<PutVacancyRequest>();
        
        // act
        var response = await Measure.ThisAsync(async () => await Client.PutAsJsonAsync($"{RouteNames.Vacancies}/{id}", request));
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();

        // assert
        vacancy.Should().BeEquivalentTo(request, opt => opt
            .Excluding(x => x.SubmittedByUserId)
            .Excluding(x => x.ReviewRequestedByUserId)
            .Excluding(x => x.ReviewRequestedByUserId)
            .Excluding(x=>x.Wage!.ApprenticeMinimumWage)
            .Excluding(x=>x.Wage!.Under18NationalMinimumWage)
            .Excluding(x=>x.Wage!.Between18AndUnder21NationalMinimumWage)
            .Excluding(x=>x.Wage!.Between21AndUnder25NationalMinimumWage)
            .Excluding(x=>x.Wage!.Over25NationalMinimumWage)
            .Excluding(x=>x.Wage!.WageText)
        );
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location.ToString().Should().Be($"/{RouteNames.Vacancies}/{vacancy.Id}");

        var record = await DbData.Get<VacancyEntity>(id);
        record.Should().BeEquivalentTo(request.ToDomain(id), opt => opt
            .Excluding(x => x.SubmittedByUserId)
            .Excluding(x => x.ReviewRequestedByUserId)
        );
    }
    
    [Test]
    public async Task Then_The_Vacancy_Is_Replaced()
    {
        // arrange
        var items = await DbData.CreateMany<VacancyEntity>(10);
        var targetItem = items[5];
        var request = Fixture.Create<PutVacancyRequest>();
        
        // act
        var response = await Measure.ThisAsync(async () => await Client.PutAsJsonAsync($"{RouteNames.Vacancies}/{targetItem.Id}", request));
        var vacancy = await response.Content.ReadAsAsync<Vacancy>();
    
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        vacancy.Should().BeEquivalentTo(request, opt => opt
            .Excluding(x => x.SubmittedByUserId)
            .Excluding(x => x.ReviewRequestedByUserId)
            .Excluding(x => x.ReviewRequestedByUserId)
            .Excluding(x=>x.Wage!.ApprenticeMinimumWage)
            .Excluding(x=>x.Wage!.Under18NationalMinimumWage)
            .Excluding(x=>x.Wage!.Between18AndUnder21NationalMinimumWage)
            .Excluding(x=>x.Wage!.Between21AndUnder25NationalMinimumWage)
            .Excluding(x=>x.Wage!.Over25NationalMinimumWage)
            .Excluding(x=>x.Wage!.WageText)
        );

        var record = await DbData.Get<VacancyEntity>(targetItem.Id);
        record.Should().BeEquivalentTo(request.ToDomain(targetItem.Id));
    }
}