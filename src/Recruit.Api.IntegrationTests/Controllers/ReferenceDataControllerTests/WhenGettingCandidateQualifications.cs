using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Core;
using SFA.DAS.Recruit.Api.Testing.Http;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.ReferenceDataControllerTests;

public class WhenGettingCandidateQualifications: BaseFixture
{
    [Test, MoqAutoData]
    public async Task Then_The_Qualifications_Are_Returned([Greedy] ReferenceDataController sut)
    {
        // arrange
        List<string> expectedValues = [
            "A Level",
            "BTEC",
            "Degree",
            "GCSE",
            "Other",
            "T Level",
        ]; 

        // act
        var response = await Client.GetAsync($"{RouteNames.ReferenceData}/candidate-qualifications");
        var qualifications = await response.Content.ReadAsAsync<List<string>>();
        
        // assert
        qualifications.Should().BeEquivalentTo(expectedValues);
    }
}