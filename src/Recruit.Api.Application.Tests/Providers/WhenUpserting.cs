using Newtonsoft.Json;
using Recruit.Api.Application.Providers;
using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Data.Models;
using Recruit.Api.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Application.Tests.Providers
{
    [TestFixture]
    public class WhenUpserting
    {
        [Test, MoqAutoData]
        public async Task Upsert_Returns_Repository_Value(
            List<ApplicationReviewEntity> entities,
            ApplicationReviewEntity existingEntity,
            [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
            [Greedy] ApplicationReviewsProvider provider)
        {
            // Arrange
            var json = JsonConvert.SerializeObject(entities);

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