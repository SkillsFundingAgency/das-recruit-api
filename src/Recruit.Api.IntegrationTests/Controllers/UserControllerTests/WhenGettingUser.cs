using System.Net;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
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
        NotificationPreferenceDefaults.Update(expected);
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
        user.Should().BeEquivalentTo(expected, opt => opt.ExcludingMissingMembers());
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
        NotificationPreferenceDefaults.Update(expected1);
        NotificationPreferenceDefaults.Update(expected2);
        
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
        var response = await Client.GetAsync($"{RouteNames.User}/by/employerAccountId/123");
        var users = await response.Content.ReadAsAsync<List<RecruitUser>>();
    
        // assert
        response.EnsureSuccessStatusCode();
        users.Should().NotBeNull();
        users.Should().HaveCount(2);
        users.Should().ContainEquivalentOf(expected1, opt => opt.ExcludingMissingMembers());
        users.Should().ContainEquivalentOf(expected2, opt => opt.ExcludingMissingMembers());
    }
    
    [TestCase(NotificationFrequency.NotSet)]
    [TestCase(NotificationFrequency.Immediately)]
    [TestCase(NotificationFrequency.Daily)]
    [TestCase(NotificationFrequency.Weekly)]
    public async Task Then_Users_Are_Filtered_When_Searching_By_EmployerAccountId_For_An_Event(NotificationFrequency frequency)
    {
        // arrange
        var items = Fixture.CreateMany<UserEntity>(3).ToList();
        items[0].NotificationPreferences = new NotificationPreferences {
            EventPreferences = [new NotificationPreference(NotificationTypes.ApplicationSubmitted, "", NotificationScope.NotSet, NotificationFrequency.Never)]
        };
        items[1].NotificationPreferences = new NotificationPreferences {
            EventPreferences = [new NotificationPreference(NotificationTypes.ApplicationSubmitted, "", NotificationScope.NotSet, frequency)]
        };
        items[2].NotificationPreferences = new NotificationPreferences {
            EventPreferences = [new NotificationPreference(NotificationTypes.ApplicationSubmitted, "", NotificationScope.NotSet, frequency)]
        };
        
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(items);
        Server.DataContext
            .Setup(x => x.UserEmployerAccountEntities)
            .ReturnsDbSet([
                new UserEmployerAccountEntity { UserId = items[0].Id, EmployerAccountId = 123, User = items[0] },
                new UserEmployerAccountEntity { UserId = items[1].Id, EmployerAccountId = 123, User = items[1] },
                new UserEmployerAccountEntity { UserId = items[2].Id, EmployerAccountId = 123, User = items[2] },
            ]);

        // act
        var response = await Client.GetAsync($"{RouteNames.User}/by/employerAccountId/123?notificationType={nameof(NotificationTypes.ApplicationSubmitted)}");
        var users = await response.Content.ReadAsAsync<List<RecruitUser>>();
    
        // assert
        response.EnsureSuccessStatusCode();
        users.Should().NotBeNull();
        users.Should().HaveCount(2);
        users.Should().ContainEquivalentOf(items[1], opt => opt.ExcludingMissingMembers());
        users.Should().ContainEquivalentOf(items[2], opt => opt.ExcludingMissingMembers());
    }
    
    [Test]
    public async Task Then_Users_Are_Found_When_Searching_By_Ukprn()
    {
        // arrange
        var items = Fixture.CreateMany<UserEntity>(10).ToList();
        var expected1 = items[1];
        var expected2 = items[2];
        NotificationPreferenceDefaults.Update(expected1);
        NotificationPreferenceDefaults.Update(expected2);
        expected1.Ukprn = 999999;
        expected2.Ukprn = 999999;
        
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(items);

        // act
        var response = await Client.GetAsync($"{RouteNames.User}/by/ukprn/999999");
        var users = await response.Content.ReadAsAsync<List<RecruitUser>>();
    
        // assert
        response.EnsureSuccessStatusCode();
        users.Should().NotBeNull();
        users.Should().HaveCount(2);
        users.Should().ContainEquivalentOf(expected1, opt => opt.ExcludingMissingMembers());
        users.Should().ContainEquivalentOf(expected2, opt => opt.ExcludingMissingMembers());
    }
    
    [TestCase(NotificationFrequency.NotSet)]
    [TestCase(NotificationFrequency.Immediately)]
    [TestCase(NotificationFrequency.Daily)]
    [TestCase(NotificationFrequency.Weekly)]
    public async Task Then_Users_Are_Filtered_When_Searching_By_Ukprn_For_An_Event(NotificationFrequency frequency)
    {
        // arrange
        var items = Fixture.CreateMany<UserEntity>(3).ToList();
        items[0].Ukprn = 999999;
        items[1].Ukprn = 999999;
        items[2].Ukprn = 999999;
        items[0].NotificationPreferences = new NotificationPreferences {
            EventPreferences = [new NotificationPreference(NotificationTypes.ApplicationSubmitted, "", NotificationScope.NotSet, NotificationFrequency.Never)]
        };
        items[1].NotificationPreferences = new NotificationPreferences {
            EventPreferences = [new NotificationPreference(NotificationTypes.ApplicationSubmitted, "", NotificationScope.NotSet, frequency)]
        };
        items[2].NotificationPreferences = new NotificationPreferences {
            EventPreferences = [new NotificationPreference(NotificationTypes.ApplicationSubmitted, "", NotificationScope.NotSet, frequency)]
        };
        
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(items);

        // act
        var response = await Client.GetAsync($"{RouteNames.User}/by/ukprn/999999?notificationType={nameof(NotificationTypes.ApplicationSubmitted)}");
        var users = await response.Content.ReadAsAsync<List<RecruitUser>>();
    
        // assert
        response.EnsureSuccessStatusCode();
        users.Should().NotBeNull();
        users.Should().HaveCount(2);
        users.Should().ContainEquivalentOf(items[1], opt => opt.ExcludingMissingMembers());
        users.Should().ContainEquivalentOf(items[2], opt => opt.ExcludingMissingMembers());
    }
    
    [Test]
    public async Task Then_Users_Are_Found_When_Searching_By_IdamsId()
    {
        // arrange
        var items = Fixture.CreateMany<UserEntity>(10).ToList();
        var expected1 = items[1];
        NotificationPreferenceDefaults.Update(expected1);
        expected1.IdamsUserId = Fixture.Create<string>();
        
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(items);
        Server.DataContext
            .Setup(x => x.UserEmployerAccountEntities)
            .ReturnsDbSet([]);

        // act
        var response = await Client.GetAsync($"{RouteNames.User}/by/idams/{expected1.IdamsUserId}");
        var user = await response.Content.ReadAsAsync<RecruitUser>();
    
        // assert
        response.EnsureSuccessStatusCode();
        user.Should().NotBeNull();
        user.Should().BeEquivalentTo(expected1, opt => opt.ExcludingMissingMembers());
    }
    
    [Test]
    public async Task Then_Users_Are_Found_When_Searching_By_DfeUserId()
    {
        // arrange
        var items = Fixture.CreateMany<UserEntity>(10).ToList();
        var expected1 = items[1];
        NotificationPreferenceDefaults.Update(expected1);
        expected1.DfEUserId = Fixture.Create<string>();
        
        Server.DataContext
            .Setup(x => x.UserEntities)
            .ReturnsDbSet(items);
        Server.DataContext
            .Setup(x => x.UserEmployerAccountEntities)
            .ReturnsDbSet([]);

        // act
        var response = await Client.GetAsync($"{RouteNames.User}/by/dfeuserid/{expected1.DfEUserId}");
        var user = await response.Content.ReadAsAsync<RecruitUser>();
    
        // assert
        response.EnsureSuccessStatusCode();
        user.Should().NotBeNull();
        user.Should().BeEquivalentTo(expected1, opt => opt.ExcludingMissingMembers());
    }
}