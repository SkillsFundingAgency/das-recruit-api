using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;

namespace SFA.DAS.Recruit.Api.UnitTests.Extensions;

internal class WhenMappingPatchRequest
{
    [Test, MoqAutoData]
    public void To_Entity_Then_The_Entity_Is_Mapped(JsonPatchDocument<ApplicationReview> source)
    {
        // act
        var result = source.ToEntity();
        
        // assert
        result.Should().BeEquivalentTo(source);
    }
}