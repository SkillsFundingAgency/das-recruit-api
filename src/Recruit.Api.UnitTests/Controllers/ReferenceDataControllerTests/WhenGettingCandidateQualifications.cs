using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ReferenceDataControllerTests;

public class WhenGettingCandidateQualifications
{
    [Test, MoqAutoData]
    public void Then_The_Skills_Are_Returned([Greedy] ReferenceDataController sut)
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
        var result = sut.GetCandidateQualifications() as Ok<List<string>>;
        var payload = result?.Value;

        // assert
        payload.Should().NotBeNull();
        payload.Should().HaveCount(expectedValues.Count);
        payload.Should().BeEquivalentTo(expectedValues);
    }
}