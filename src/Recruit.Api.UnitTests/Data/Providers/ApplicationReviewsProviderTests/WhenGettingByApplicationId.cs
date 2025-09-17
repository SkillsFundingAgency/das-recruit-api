using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Providers.ApplicationReviewsProviderTests;

internal class WhenGettingByApplicationId
{
    [Test, MoqAutoData]
    public async Task GetByApplicationId_ShouldReturnEntity_WhenEntityExists(
        Guid applicationId,
        ApplicationReviewEntity entity,
        [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
        [Greedy] ApplicationReviewsProvider provider)
    {
        // Arrange
        entity.ApplicationId = applicationId;
        repositoryMock.Setup(r => r.GetByApplicationId(applicationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await provider.GetByApplicationId(applicationId);

        // Assert
        result.Should().BeEquivalentTo(entity);
    }

    [Test, MoqAutoData]
    public async Task GetById_ShouldReturnNull_WhenEntityDoesNotExist(
        ApplicationReviewEntity entity,
        [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
        [Greedy] ApplicationReviewsProvider provider)
    {
        // Arrange
        repositoryMock.Setup(r => r.GetByApplicationId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationReviewEntity?) null);

        // Act
        var result = await provider.GetByApplicationId(It.IsAny<Guid>());

        // Assert
        result.Should().BeNull();
    }
}