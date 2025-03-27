using Recruit.Api.Application.Providers;
using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Application.Tests.Providers;

[TestFixture]
public class WhenGettingById
{
    [Test, MoqAutoData]
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

    [Test, MoqAutoData]
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