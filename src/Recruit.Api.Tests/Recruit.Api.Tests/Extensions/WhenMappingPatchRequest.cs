using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.Recruit.Api.Extensions;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Tests.Extensions;

public class WhenMappingPatchRequest
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