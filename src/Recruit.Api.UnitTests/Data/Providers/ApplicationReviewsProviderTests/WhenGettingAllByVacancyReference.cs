using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Providers.ApplicationReviewsProviderTests
{
    [TestFixture]
    internal class WhenGettingAllByVacancyReference
    {
        [Test, RecursiveMoqAutoData]
        public async Task GettingAllByVacancyReference_ShouldReturnApplicationReviews(
            long vacancyReference,
            CancellationToken token,
            List<ApplicationReviewEntity> entities,
            [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
            [Greedy] ApplicationReviewsProvider provider)
        {
            // Arrange
            repositoryMock.Setup(repo => repo.GetAllByVacancyReference(vacancyReference, token))
                .ReturnsAsync(entities);
            
            // Act
            var result = await provider.GetAllByVacancyReference(vacancyReference, token);
            
            // Assert
            result.Should().BeEquivalentTo(entities);
        }
    }
}