using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.ApplicationReviewRepositoryTests;
[TestFixture]
internal class WhenGettingApplicationsByIds
{
    [Test, RecursiveMoqAutoData]
    public async Task GettingApplicationReviewsByIds(
        Guid applicationId,
        CancellationToken token,
        ApplicationReviewEntity entity,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] ApplicationReviewRepository repository)
    {
        // Arrange
        entity.Id = applicationId;
        context.Setup(x => x.ApplicationReviewEntities)
            .ReturnsDbSet([entity]);
        // Act
        var result = await repository.GetAllByIdAsync([applicationId], CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo([entity]);
    }
}