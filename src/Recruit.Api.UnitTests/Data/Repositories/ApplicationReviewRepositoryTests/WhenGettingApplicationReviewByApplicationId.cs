using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Testing.Data;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ApplicationReviewRepositoryTests;

internal class WhenGettingApplicationReviewByApplicationId
{
    [Test, RecursiveMoqAutoData]
    public async Task GetById_ShouldReturnEntity_WhenEntityExists(
        Guid applicationId,
        ApplicationReviewEntity expectedEntity,
        [Frozen] Mock<IRecruitDataContext> mockContext,
        [Greedy] ApplicationReviewRepository repository)
    {
        // Arrange
        expectedEntity.ApplicationId = applicationId;
        mockContext.Setup(m => m.ApplicationReviewEntities).ReturnsDbSet(new List<ApplicationReviewEntity>{ expectedEntity });

        // Act
        var result = await repository.GetByApplicationId(applicationId);

        // Assert
        result.Should().BeEquivalentTo(expectedEntity);
    }

    [Test, RecursiveMoqAutoData]
    public async Task GetById_ShouldReturnNull_WhenEntityDoesNotExist(
        Guid applicationId,
        [Frozen] Mock<IRecruitDataContext> mockContext,
        [Greedy] ApplicationReviewRepository repository)
    {
        // Arrange
        mockContext.Setup(m => m.ApplicationReviewEntities).ReturnsDbSet(new List<ApplicationReviewEntity>());

        // Act
        var result = await repository.GetByApplicationId(applicationId);

        // Assert
        result.Should().BeNull();
    }
}