using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Testing.Data;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ApplicationReviewRepositoryTests;

internal class WhenUpdatingApplicationReview
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_When_ApplicationReview_Not_Found_By_Id_Updated_By_Application_Review_Id(
        ApplicationReviewEntity updateEntity,
        ApplicationReviewEntity entity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        ApplicationReviewRepository repository)
    {
        entity.Id = Guid.NewGuid();
        entity.ApplicationId = updateEntity.ApplicationId;
        dataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet([entity]);
        
        var actual = await repository.Update(updateEntity);
        
        dataContext.Verify(x=>x.SetValues(entity, updateEntity), Times.Once);
        actual!.Id.Should().Be(entity.Id);
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_When_ApplicationReview_Not_Found_By_Id_Or_Application_Id_Null_Returned(
        ApplicationReviewEntity updateEntity,
        ApplicationReviewEntity entity,
        [Frozen] Mock<IRecruitDataContext> dataContext,
        ApplicationReviewRepository repository)
    {
        entity.Id = Guid.NewGuid();
        dataContext.Setup(x => x.ApplicationReviewEntities).ReturnsDbSet([entity]);
        
        var actual = await repository.Update(updateEntity);
        
        dataContext.Verify(x=>x.SetValues(It.IsAny<ApplicationReviewEntity>(), It.IsAny<ApplicationReviewEntity>()), Times.Never);
        actual.Should().BeNull();
    }
}