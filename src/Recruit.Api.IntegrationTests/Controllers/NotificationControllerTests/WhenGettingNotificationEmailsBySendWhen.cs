using System.Net;
using System.Text.Encodings.Web;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Responses.Notifications;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.NotificationControllerTests;

public class WhenGettingNotificationEmailsBySendWhen : BaseFixture
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
            User = new UserEntity {
                    Email = "Email value",
                    Name = "Some name",
            },
            SendWhen = DateTime.Now.AddDays(-1),
            EmailTemplateId = new Guid("f6fc57e6-7318-473d-8cb5-ca653035391a"), // this is a dev template
            StaticData = "{\"firstName\":\"Fred\",\"trainingProvider\":\"Fred\",\"vacancyReference\":\"1001\",\"applicationUrl\":\"Fred\"}",
            DynamicData = "{}"
        },
        new() {
            Id = 3,
            UserId = Guid.NewGuid(),
            SendWhen = DateTime.Now.AddDays(2),
            EmailTemplateId = Guid.NewGuid(),
            StaticData = "{\"email\":\"Email value\"}",
            DynamicData = "{\"title\":\"Some title\"}"
        },
    ];
    
    [Test]
    public async Task Then_The_Notifications_In_The_Past_Are_Returned()
    {
        // arrange
        Server.DataContext.Setup(x => x.RecruitNotifications).ReturnsDbSet(Items);

        // act
        var response = await Client.GetAsync($"{RouteNames.Notifications}/batch/by/sendwhen/{UrlEncoder.Default.Encode(DateTime.Now.ToString("s"))}");
        var results = await response.Content.ReadAsAsync<GetBatchByDateResponse>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        results.Should().NotBeNull();
        results.Emails.Should().HaveCount(1);
        results.Ids.Should().HaveCount(1);
        results.Ids.Should().Contain(2);
    }
}