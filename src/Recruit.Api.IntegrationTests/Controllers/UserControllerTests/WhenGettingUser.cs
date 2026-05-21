using System.Net;
using SFA.DAS.Recruit.Api.Domain;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Contracts.ApiRequests;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.UserControllerTests;

public class WhenGettingUser: MsSqlBaseFixture
{
    [Test]
    public async Task Then_The_User_Is_Returned()
    {
        // arrange
        var items = await DbData.CreateMany<UserEntity>(10);
        var expected = items[new Random().Next(items.Count)];
        NotificationPreferenceDefaults.Update(expected);

        // act
        var response = await Client.GetAsync(new GetUserByIdApiRequest(expected.Id).GetUrl);
        var user = await response.Content.ReadAsAsync<RecruitUser>();
    
        // assert
        response.EnsureSuccessStatusCode();
        user.Should().NotBeNull();
        user.Should().BeEquivalentTo(expected, opt => opt.ExcludingMissingMembers());
    }
    
    [Test]
    public async Task Then_The_User_Is_NotFound()
    {
        // act
        var response = await Client.GetAsync(new GetUserByIdApiRequest(Guid.NewGuid()).GetUrl);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task Then_Users_Are_Found_When_Searching_By_EmployerAccountId()
    {
        // arrange
        var employerAccountId = EmployerAccountIdGenerator.GetNext();
        var items = await DbData.CreateMany<UserEntity>(10, x =>
        {
            NotificationPreferenceDefaults.Update(x);
            x[1].EmployerAccounts[0].EmployerAccountId = employerAccountId;
            x[1].EmployerAccounts[1].EmployerAccountId = EmployerAccountIdGenerator.GetNext();
            x[2].EmployerAccounts[0].EmployerAccountId = employerAccountId;
        });
        var expected1 = items[1];
        var expected2 = items[2];
        
        // act
        var response = await Client.GetAsync(new GetUserByEmployeraccountidByEmployerAccountIdApiRequest(123, null).GetUrl);
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
        var employerAccountId = EmployerAccountIdGenerator.GetNext();
        var items = await DbData.CreateMany<UserEntity>(3, x =>
        {
            x[0].NotificationPreferences = new NotificationPreferences {
                EventPreferences = [new NotificationPreference(NotificationTypes.ApplicationSubmitted, "", NotificationScope.NotSet, NotificationFrequency.Never)]
            };
            x[1].NotificationPreferences = new NotificationPreferences {
                EventPreferences = [new NotificationPreference(NotificationTypes.ApplicationSubmitted, "", NotificationScope.NotSet, frequency)]
            };
            x[2].NotificationPreferences = new NotificationPreferences {
                EventPreferences = [new NotificationPreference(NotificationTypes.ApplicationSubmitted, "", NotificationScope.NotSet, frequency)]
            };
            
            NotificationPreferenceDefaults.Update(x);

            x[0].EmployerAccounts[0].EmployerAccountId = employerAccountId;
            x[1].EmployerAccounts[0].EmployerAccountId = employerAccountId;
            x[2].EmployerAccounts[0].EmployerAccountId = employerAccountId;
        });
        
        // act
        var response = await Client.GetAsync(new GetUserByEmployeraccountidByEmployerAccountIdApiRequest(123, SFA.DAS.Recruit.Contracts.ApiResponses.NotificationTypes.ApplicationSubmitted).GetUrl);
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
        var ukprn = UkprnGenerator.GetNext();
        var items = await DbData.CreateMany<UserEntity>(10, x =>
        {
            NotificationPreferenceDefaults.Update(x);
            x[1].Ukprn = ukprn;
            x[2].Ukprn = ukprn;
        });
        
        var expected1 = items[1];
        var expected2 = items[2];
    
        // act
        var response = await Client.GetAsync(new GetUserByUkprnByUkprnApiRequest(999999, null).GetUrl);
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
        var ukprn = UkprnGenerator.GetNext();
        var items = await DbData.CreateMany<UserEntity>(3, x =>
        {
            foreach (var user in x)
            {
                user.Ukprn = ukprn;
            }
            
            x[0].NotificationPreferences = new NotificationPreferences {
                EventPreferences = [new NotificationPreference(NotificationTypes.ApplicationSubmitted, "", NotificationScope.NotSet, NotificationFrequency.Never)]
            };
            x[1].NotificationPreferences = new NotificationPreferences {
                EventPreferences = [new NotificationPreference(NotificationTypes.ApplicationSubmitted, "", NotificationScope.NotSet, frequency)]
            };
            x[2].NotificationPreferences = new NotificationPreferences {
                EventPreferences = [new NotificationPreference(NotificationTypes.ApplicationSubmitted, "", NotificationScope.NotSet, frequency)]
            };
            
            NotificationPreferenceDefaults.Update(x);
        });
    
        // act
        var response = await Client.GetAsync(new GetUserByUkprnByUkprnApiRequest(999999, SFA.DAS.Recruit.Contracts.ApiResponses.NotificationTypes.ApplicationSubmitted).GetUrl);
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
        var items = await DbData.CreateMany<UserEntity>(3, NotificationPreferenceDefaults.Update);
        var expected1 = items[1];
    
        // act
        var response = await Client.GetAsync(new GetUserByIdamsByIdamsApiRequest(expected1.IdamsUserId).GetUrl);
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
        var items = await DbData.CreateMany<UserEntity>(3, NotificationPreferenceDefaults.Update);
        var expected1 = items[1];
    
        // act
        var response = await Client.GetAsync(new GetUserByDfeuseridByDfeUserIdApiRequest(expected1.DfEUserId).GetUrl);
        var user = await response.Content.ReadAsAsync<RecruitUser>();
    
        // assert
        response.EnsureSuccessStatusCode();
        user.Should().NotBeNull();
        user.Should().BeEquivalentTo(expected1, opt => opt.ExcludingMissingMembers());
    }
}
