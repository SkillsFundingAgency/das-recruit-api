using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Mappers;

namespace SFA.DAS.Recruit.Api.UnitTests.Extensions;

internal class WhenMappingApplicationReviewEntity
{
    [Test, MoqAutoData]
    public void To_GetApplicationReviewResponse_Then_The_Entity_Is_Mapped(ApplicationReviewEntity entity)
    {
        // act
        var result = entity.ToGetResponse();
        
        // assert
        result.Should().BeEquivalentTo(entity);
    }
    
    [Test, MoqAutoData]
    public void To_PatchApplicationReviewResponse_Then_The_Entity_Is_Mapped(ApplicationReviewEntity entity)
    {
        // act
        var result = entity.ToPatchResponse();
        
        // assert
        result.Should().BeEquivalentTo(entity);
    }
    
    [Test, MoqAutoData]
    public void To_PutApplicationReviewResponse_Then_The_Entity_Is_Mapped(ApplicationReviewEntity entity)
    {
        // act
        var result = entity.ToPutResponse();
        
        // assert
        result.Should().BeEquivalentTo(entity);
    }
    
    [Test, MoqAutoData]
    public void To_ApplicationReviewResponse_Then_The_Entity_Is_Mapped(ApplicationReviewEntity entity)
    {
        // act
        var result = entity.ToApplicationReview();
        
        // assert
        result.Should().BeEquivalentTo(entity);
    }
}