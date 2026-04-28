using System.Text.Json;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;

namespace SFA.DAS.Recruit.Api.UnitTests.Extensions;

internal class WhenMappingPatchRequest
{
    [Test, MoqAutoData]
    public void To_Entity_Then_The_Entity_Is_Mapped(Operation<ApplicationReview> data)
    {
        var source = new JsonPatchDocument<ApplicationReview>([data], new JsonSerializerOptions()); 
        
        // act
        var result = source.ToEntity();
        
        // assert
        result.Operations.Should().BeEquivalentTo(source.Operations);
    }
}