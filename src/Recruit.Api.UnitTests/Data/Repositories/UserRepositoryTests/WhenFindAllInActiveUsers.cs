using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.UserRepositoryTests;

[TestFixture]
internal class WhenFindAllInActiveUsers
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_Inactive_Users_Are_Returned(
        List<UserEntity> entities,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] UserRepository sut,
        CancellationToken token)
    {
        // arrange
        foreach (UserEntity userEntity in entities)
        {
            userEntity.LastSignedInDate = DateTime.UtcNow.AddDays(-1);
        }
        context.Setup(x => x.UserEntities).ReturnsDbSet(entities);

        // act
        var result = await sut.FindAllInActiveUsersAsync(token);

        // assert
        result.Should().NotBeNullOrEmpty();
        context.Verify(x => x.UserEntities, Times.Once);
        result.Count.Should().Be(entities.Count);
        result.Should().BeEquivalentTo(entities);
        result.All(v => v.LastSignedInDate > DateTime.UtcNow.AddYears(-1)).Should().BeTrue();
    }
}