using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Core;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.ReferenceDataControllerTests;

public class WhenGettingCandidateSkills: BaseFixture
{
    [Test, MoqAutoData]
    public async Task Then_The_Skills_Are_Returned([Greedy] ReferenceDataController sut)
    {
        // arrange
        List<string> expectedValues = [
            "Administrative skills",
            "Analytical skills",
            "Attention to detail",
            "Communication skills",
            "Creative",
            "Customer care skills",
            "IT skills",
            "Initiative",
            "Logical",
            "Non judgemental",
            "Number skills",
            "Organisation skills",
            "Patience",
            "Physical fitness",
            "Presentation skills",
            "Problem solving skills",
            "Team working",
        ]; 

        // act
        var response = await Client.GetAsync($"{RouteNames.ReferenceData}/candidate-skills");
        var skills = await response.Content.ReadAsAsync<List<string>>();

        // assert
        skills.Should().BeEquivalentTo(expectedValues);
    }
}