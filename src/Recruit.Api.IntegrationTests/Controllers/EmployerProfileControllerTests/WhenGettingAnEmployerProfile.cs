using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Testing.Data;
using SFA.DAS.Recruit.Api.Testing.Http;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.EmployerProfileControllerTests;

public class WhenGettingAnEmployerProfile : BaseFixture
{
    [Test]
    public async Task Then_The_Profile_Is_Returned()
    {
        // arrange
        List<EmployerProfileEntity> items = [
            new() { AccountLegalEntityId = 1, AccountId = 10 },
            new() { AccountLegalEntityId = 2, AccountId = 11 },
            new() { AccountLegalEntityId = 3, AccountId = 12 },
        ];
        Server.DataContext.Setup(x => x.EmployerProfileEntities).ReturnsDbSet(items);

        // act
        var response = await Client.GetAsync($"{RouteNames.EmployerProfile}/2");
        var employerProfile = await response.Content.ReadAsAsync<EmployerProfile>();

        // assert
        response.EnsureSuccessStatusCode();
        employerProfile.Should().BeEquivalentTo(items[1], options => options.ExcludingMissingMembers());
    }
    
    [Test]
    public async Task Then_The_Profiles_Are_Returned()
    {
        // arrange
        List<EmployerProfileEntity> items = [
            new() { AccountLegalEntityId = 1, AccountId = 10 },
            new() { AccountLegalEntityId = 2, AccountId = 11 },
            new() { AccountLegalEntityId = 3, AccountId = 12 },
            new() { AccountLegalEntityId = 4, AccountId = 12 },
        ];
        Server.DataContext.Setup(x => x.EmployerProfileEntities).ReturnsDbSet(items);

        // act
        var response = await Client.GetAsync($"/{RouteNames.Employer}/12/{RouteElements.EmployerProfiles}");
        var employerProfiles = await response.Content.ReadAsAsync<List<EmployerProfile>>();

        // assert
        response.EnsureSuccessStatusCode();
        employerProfiles.Should().BeEquivalentTo(items.Skip(2), options => options.ExcludingMissingMembers());
    }
}