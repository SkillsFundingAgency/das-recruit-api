using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models.Mappers;

namespace SFA.DAS.Recruit.Api.UnitTests.Extensions;

internal class WhenMappingApplicationReviewEntity
{
    [Test, RecursiveMoqAutoData]
    public void To_GetApplicationReviewResponse_Then_The_Entity_Is_Mapped(ApplicationReviewEntity entity)
    {
        //Arrange
        entity.Status = ApplicationReviewStatus.InReview;
        
        // act
        var result = entity.ToGetResponse();
        
        // assert
        result.Should().BeEquivalentTo(entity, options => options.Excluding(c=>c.Status));
        result.Status.Should().Be(ApplicationReviewStatus.InReview);
    }
    
    [Test, RecursiveMoqAutoData]
    public void To_PatchApplicationReviewResponse_Then_The_Entity_Is_Mapped(ApplicationReviewEntity entity)
    {
        //Arrange
        entity.Status = ApplicationReviewStatus.InReview;
        
        // act
        var result = entity.ToPatchResponse();
        
        // assert
        result.Should().BeEquivalentTo(entity, options => options.Excluding(c=>c.Status));
        result.Status.Should().Be(ApplicationReviewStatus.InReview);
    }
    
    [Test, RecursiveMoqAutoData]
    public void To_ApplicationReviewResponse_Then_The_Entity_Is_Mapped(ApplicationReviewEntity entity)
    {
        //Arrange
        entity.Status = ApplicationReviewStatus.InReview;
        
        // act
        var result = entity.ToGetResponse();
        
        // assert
        result.Should().BeEquivalentTo(entity, options => options.Excluding(c=>c.Status));
        result.Status.Should().Be(ApplicationReviewStatus.InReview);
    }
}