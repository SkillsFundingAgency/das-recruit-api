using System.Net;
using System.Text.Json;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.UserControllerTests;

public class WhenGettingUser: BaseFixture
{
    [Test]
    public async Task Then_The_User_Is_Returned()
    {
        // arrange
        var items = Fixture.CreateMany<UserEntity>(10).ToList();
        var expected = items[1];
        expected.NotificationPreferences = JsonSerializer.Serialize(Fixture.Create<NotificationPreferences>());
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(items);
        Server.DataContext
            .Setup(x => x.UserEmployerAccountEntities)
            .ReturnsDbSet([]);

        // act
        var response = await Client.GetAsync($"{RouteNames.User}/{expected.Id}");
        var user = await response.Content.ReadAsAsync<RecruitUser>();
    
        // assert
        response.EnsureSuccessStatusCode();
        user.Should().NotBeNull();
        user.Should().BeEquivalentTo(expected, opt => opt.ExcludingMissingMembers().Excluding(x => x.NotificationPreferences));
    }
    
    [Test]
    public async Task Then_The_User_Is_NotFound()
    {
        // arrange
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(Fixture.CreateMany<UserEntity>(10).ToList());
        Server.DataContext
            .Setup(x => x.UserEmployerAccountEntities)
            .ReturnsDbSet([]);

        // act
        var response = await Client.GetAsync($"{RouteNames.User}/{Guid.NewGuid()}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task Then_Users_Are_Found_When_Searching_By_EmployerAccountId()
    {
        // arrange
        var items = Fixture.CreateMany<UserEntity>(10).ToList();
        var expected1 = items[1];
        var expected2 = items[2];
        
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(items);
        Server.DataContext
            .Setup(x => x.UserEmployerAccountEntities)
            .ReturnsDbSet([
                new UserEmployerAccountEntity { UserId = expected1.Id, EmployerAccountId = 123, User = expected1 },
                new UserEmployerAccountEntity { UserId = expected1.Id, EmployerAccountId = 456, User = expected1 },
                new UserEmployerAccountEntity { UserId = expected2.Id, EmployerAccountId = 123, User = expected2 },
            ]);

        // act
        var response = await Client.GetAsync($"{RouteNames.User}/by/employerAccountId/ABCD");
        var users = await response.Content.ReadAsAsync<List<RecruitUser>>();
    
        // assert
        response.EnsureSuccessStatusCode();
        users.Should().NotBeNull();
        users.Should().HaveCount(2);
        users.Should().ContainEquivalentOf(expected1, opt => opt.ExcludingMissingMembers().Excluding(x => x.NotificationPreferences));
        users.Should().ContainEquivalentOf(expected2, opt => opt.ExcludingMissingMembers().Excluding(x => x.NotificationPreferences));
    }
    
    [Test]
    public async Task Then_Users_Are_Found_When_Searching_By_Ukprn()
    {
        // arrange
        var items = Fixture.CreateMany<UserEntity>(10).ToList();
        var expected1 = items[1];
        var expected2 = items[2];
        expected1.Ukprn = 999999;
        expected2.Ukprn = 999999;
        
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(items);
        Server.DataContext
            .Setup(x => x.UserEmployerAccountEntities)
            .ReturnsDbSet([]);

        // act
        var response = await Client.GetAsync($"{RouteNames.User}/by/ukprn/999999");
        var users = await response.Content.ReadAsAsync<List<RecruitUser>>();
    
        // assert
        response.EnsureSuccessStatusCode();
        users.Should().NotBeNull();
        users.Should().HaveCount(2);
        users.Should().ContainEquivalentOf(expected1, opt => opt.ExcludingMissingMembers().Excluding(x => x.NotificationPreferences));
        users.Should().ContainEquivalentOf(expected2, opt => opt.ExcludingMissingMembers().Excluding(x => x.NotificationPreferences));
    }
}