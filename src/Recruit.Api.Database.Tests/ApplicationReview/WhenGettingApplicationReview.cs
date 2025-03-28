using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Recruit.Api.Data;
using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Database.Tests.DatabaseMock;
using Recruit.Api.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Database.Tests.ApplicationReview
{
    [TestFixture]
    public class WhenGettingApplicationReview
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
}
