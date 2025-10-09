using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ApplicationReviewRepositoryTests
{
    [TestFixture]
    internal class WhenGettingAllByVacancyReference
    {
        [Test, MoqAutoData]
        public async Task GettingApplicationReviewsByVacancyReference(
            long vacancyReference,
            CancellationToken token,
            List<ApplicationReviewEntity> entities,
            [Frozen] Mock<IRecruitDataContext> context,
            [Greedy] ApplicationReviewRepository repository)
        {
            // Arrange
            foreach (var applicationReviewEntity in entities)
            {
                applicationReviewEntity.VacancyReference = vacancyReference;
            }

            context.Setup(x => x.ApplicationReviewEntities)
                .ReturnsDbSet(entities);

            // Act
            var result = await repository.GetAllByVacancyReference(vacancyReference, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(entities);
        }
    }
}