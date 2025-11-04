using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Providers.ApplicationReviewsProviderTests;

[TestFixture]
internal class WhenGettingById
{
    [Test, RecursiveMoqAutoData]
    public async Task GetById_ShouldReturnEntity_WhenEntityExists(
        Guid id,
        ApplicationReviewEntity entity,
        [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
        [Greedy] ApplicationReviewsProvider provider)
    {
        // Arrange
        entity.Id = id;
        repositoryMock.Setup(r => r.GetById(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await provider.GetById(id);

        // Assert
        result.Should().BeEquivalentTo(entity);
    }

    [Test, RecursiveMoqAutoData]
    public async Task GetById_ShouldReturnNull_WhenEntityDoesNotExist(
        ApplicationReviewEntity entity,
        [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
        [Greedy] ApplicationReviewsProvider provider)
    {
        // Arrange
        repositoryMock.Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationReviewEntity?) null);

        // Act
        var result = await provider.GetById(It.IsAny<Guid>());

        // Assert
        result.Should().BeNull();
    }
}