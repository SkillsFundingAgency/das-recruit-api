using System.Net;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.NotificationControllerTests;

public class WhenCreatingVacancyNotifications: BaseFixture
{
    [Test]
    [MoqInlineAutoData(VacancyStatus.Draft)]
    [MoqInlineAutoData(VacancyStatus.Rejected)]
    [MoqInlineAutoData(VacancyStatus.Referred)]
    [MoqInlineAutoData(VacancyStatus.Live)]
    [MoqInlineAutoData(VacancyStatus.Closed)]
    [MoqInlineAutoData(VacancyStatus.Approved)]
    public async Task And_No_Handler_Is_Registered_For_The_Status_Then_NotImplemented_Returned(VacancyStatus status, VacancyEntity vacancy)
    {
        // arrange
        vacancy.Status = status;
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([vacancy]);
        
        // act
        var response = await Client.PostAsync($"{RouteNames.Vacancies}/{vacancy.Id}/create-notifications", null);
        var errors = await response.Content.ReadAsAsync<ProblemDetails>();
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        errors.Should().NotBeNull();
        errors.Title.Should().Be("The request could not be completed");
    }
    
    [Test, MoqAutoData]
    public async Task And_Vacancy_Does_Not_Exist_Then_BadRequest_Returned(Guid id)
    {
        // arrange
        Server.DataContext.Setup(x => x.VacancyEntities).ReturnsDbSet([]);

        // act
        var response = await Client.PostAsync($"{RouteNames.Vacancies}/{id}/create-notifications", null);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}