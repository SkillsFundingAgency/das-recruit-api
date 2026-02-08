using System.Net;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Testing.Data;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.NotificationControllerTests;

public class WhenDeletingManyRecruitNotifications : BaseFixture
{
    private static readonly List<RecruitNotificationEntity> Items = [
        new() {
            Id = 1,
            UserId = Guid.NewGuid(),
            SendWhen = DateTime.Now.AddDays(1),
            EmailTemplateId = Guid.NewGuid(),
            StaticData = "{\"email\":\"Email value\"}",
            DynamicData = "{\"title\":\"Some title\"}"
        },
        new() {
            Id = 2,
            UserId = Guid.NewGuid(),
            SendWhen = DateTime.Now.AddDays(-1),
            EmailTemplateId = Guid.NewGuid(),
            StaticData = "{\"email\":\"Email value\"}",
            DynamicData = "{\"title\":\"Some title\"}"
        },
        new() {
            Id = 3,
            UserId = Guid.NewGuid(),
            SendWhen = DateTime.Now.AddDays(2),
            EmailTemplateId = Guid.NewGuid(),
            StaticData = "{\"email\":\"Email value\"}",
            DynamicData = "{\"title\":\"Some title\"}"
        },
        new() {
            Id = 4,
            UserId = Guid.NewGuid(),
            SendWhen = DateTime.Now.AddDays(-2),
            EmailTemplateId = Guid.NewGuid(),
            StaticData = "{\"email\":\"Email value\"}",
            DynamicData = "{\"title\":\"Some title\"}"
        },
    ];
    
    // TODO: really need an actual db to test they've been removed
    // [Test]
    // public async Task Then_The_Notifications_Are_Deleted()
    // {
    //     // arrange
    //     Server.DataContext.Setup(x => x.RecruitNotifications).ReturnsDbSet(Items);
    //
    //     // act
    //     var response = await Client.DeleteAsync($"{RouteNames.Notifications}?ids=1&ids=3");
    //
    //     // assert
    //     response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    // }
    
    [Test]
    public async Task Then_Passing_No_Ids_Returns_BadRequest()
    {
        // arrange
        Server.DataContext.Setup(x => x.RecruitNotifications).ReturnsDbSet(Items);

        // act
        var response = await Client.DeleteAsync($"{RouteNames.Notifications}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}