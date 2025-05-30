﻿using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.UnitTests.Application.Providers.ApplicationReviewsProviderTests;

[TestFixture]
internal class WhenUpdating
{
    [Test, MoqAutoData]
    public async Task Update_Returns_Repository_Value(
        ApplicationReviewEntity updatedEntity,
        [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
        [Greedy] ApplicationReviewsProvider provider)
    {
        // Arrange
        repositoryMock
            .Setup(r => r.Update(It.IsAny<ApplicationReviewEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedEntity);

        // Act
        var result = await provider.Update(updatedEntity, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.Update(updatedEntity, CancellationToken.None), Times.Once);
        result.Should().Be(updatedEntity);
    }
}