using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ReferenceDataControllerTests;

public class WhenGettingCandidateSkills
{
    [Test, MoqAutoData]
    public void Then_The_Skills_Are_Returned([Greedy] ReferenceDataController sut)
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
        var result = sut.GetCandidateSkills() as Ok<List<string>>;
        var payload = result?.Value;

        // assert
        payload.Should().NotBeNull();
        payload.Should().HaveCount(expectedValues.Count);
        payload.Should().BeEquivalentTo(expectedValues);
    }
}