﻿using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.ApplicationReviewRepositoryTests;

[TestFixture]
internal class WhenGettingApplicationReview
{
    [Test, RecursiveMoqAutoData]
    public async Task GetById_ShouldReturnEntity_WhenEntityExists(
        Guid id,
        ApplicationReviewEntity expectedEntity,
        [Frozen] Mock<IRecruitDataContext> mockContext,
        [Greedy] ApplicationReviewRepository repository)
    {
        // Arrange
        expectedEntity.Id = id;
        mockContext.Setup(m => m.ApplicationReviewEntities).ReturnsDbSet(new List<ApplicationReviewEntity>{ expectedEntity });

        // Act
        var result = await repository.GetById(id);

        // Assert
        result.Should().BeEquivalentTo(expectedEntity);
    }

    [Test, RecursiveMoqAutoData]
    public async Task GetById_ShouldReturnNull_WhenEntityDoesNotExist(
        Guid id,
        ApplicationReviewEntity expectedEntity,
        [Frozen] Mock<IRecruitDataContext> mockContext,
        [Greedy] ApplicationReviewRepository repository)
    {
        // Arrange
        expectedEntity.Id = id;
        mockContext.Setup(m => m.ApplicationReviewEntities).ReturnsDbSet(new List<ApplicationReviewEntity>());

        // Act
        var result = await repository.GetById(id);

        // Assert
        result.Should().BeNull();
    }
}