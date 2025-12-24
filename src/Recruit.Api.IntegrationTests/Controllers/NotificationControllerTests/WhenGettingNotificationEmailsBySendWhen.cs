using System.Net;
using System.Text.Encodings.Web;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Responses.Notifications;
using SFA.DAS.Recruit.Api.Testing.Data;
using SFA.DAS.Recruit.Api.Testing.Http;

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
            DynamicData = "{\"title\":\"Some title\"}",
            User = new UserEntity 
            {
                Id = Guid.NewGuid(),
                Name = "someName",
                Email = "someEmail",
                LastSignedInDate = DateTime.UtcNow.AddDays(-1)
            }
        },
        new() {
            Id = 2,
            UserId = Guid.NewGuid(),
            User = new UserEntity
            {
                Email = "Email value",
                Name = "Some name",
                LastSignedInDate = DateTime.UtcNow.AddDays(-1)
            },
            SendWhen = DateTime.Now.AddDays(-1),
            EmailTemplateId = new Guid("f6fc57e6-7318-473d-8cb5-ca653035391a"), // this is a dev template
            StaticData = "{\"firstName\":\"Fred\",\"trainingProvider\":\"Fred\",\"vacancyReference\":\"1001\",\"applicationUrl\":\"Fred\"}",
            DynamicData = "{}",
        },
        new() {
            Id = 3,
            UserId = Guid.NewGuid(),
            SendWhen = DateTime.Now.AddDays(2),
            EmailTemplateId = Guid.NewGuid(),
            StaticData = "{\"email\":\"Email value\"}",
            DynamicData = "{\"title\":\"Some title\"}",
            User = new UserEntity 
            {
                Id = Guid.NewGuid(),
                Name = "someName",
                Email = "someEmail",
                LastSignedInDate = DateTime.UtcNow.AddDays(-1)
            }
        },
    ];

    [Test]
    public async Task Then_The_Notifications_In_The_Past_Are_Returned()
    {
        // arrange
        Server.DataContext.Setup(x => x.RecruitNotifications).ReturnsDbSet(Items);

        // act
        var response = await Client.GetAsync($"{RouteNames.Notifications}/batch/by/date?dateTime={UrlEncoder.Default.Encode(DateTime.Now.ToString("s"))}");
        var results = await response.Content.ReadAsAsync<GetBatchByDateResponse>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        results.Should().NotBeNull();
        results.Emails.Should().HaveCount(1);
    }
}