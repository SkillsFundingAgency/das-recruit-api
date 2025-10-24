using SFA.DAS.Recruit.Api.Data.Models;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Providers.ApplicationReviewsProviderTests
{
    [TestFixture]
    internal class WhenUpserting
    {
        [Test, RecursiveMoqAutoData]
        public async Task Upsert_Returns_Repository_Value(
            List<ApplicationReviewEntity> entities,
            ApplicationReviewEntity existingEntity,
            [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
            [Greedy] ApplicationReviewsProvider provider)
        {
            // Arrange
            var expectedTuple = UpsertResult.Create(existingEntity, true);

            repositoryMock
                .Setup(r => r.Upsert(existingEntity, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTuple);

            // Act
            var result = await provider.Upsert(existingEntity);

            // Assert
            result.Should().BeEquivalentTo(expectedTuple);
        }
    }
}