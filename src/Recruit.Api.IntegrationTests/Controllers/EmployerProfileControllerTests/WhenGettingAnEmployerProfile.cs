using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;

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
        var response = await Client.GetAsync("/api/employer/profiles/2");
        var employerProfile = await response.Content.ReadAsAsync<EmployerProfile>();

        // assert
        response.EnsureSuccessStatusCode();
        employerProfile.Should().BeEquivalentTo(items[1], options => options.ExcludingMissingMembers());
    }
}