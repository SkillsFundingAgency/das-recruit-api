using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.User;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.UserRepositoryTests;

internal class WhenUpsertingUser
{
    [Test, RecursiveMoqAutoData]
    public async Task UpsertOneAsync_Inserts_New_Entity(
        UserEntity entity,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] UserRepository sut,
        CancellationToken token)
    {
        // arrange
        var dbSet = new List<UserEntity>().BuildDbSetMock();
        context.Setup(x => x.UserEntities).Returns(dbSet.Object);

        // act
        var result = await sut.UpsertOneAsync(entity, token);

        // assert
        context.Verify(x => x.SaveChangesAsync(token), Times.Once);
        dbSet.Verify(x => x.Add(entity), Times.Once);
        result.Created.Should().BeTrue();
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task UpsertOneAsync_Updates_Existing_Entity(
        UserEntity entity,
        UserEntity entity2,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] UserRepository sut,
        CancellationToken token)
    {
        // arrange
        entity.Id = entity2.Id;
        List<UserEntity> items = [
            entity2,
        ];
        var dbSet = items.BuildDbSetMock();
        context.Setup(x => x.UserEntities).Returns(dbSet.Object);

        // act
        var result = await sut.UpsertOneAsync(entity, token);

        // assert
        context.Verify(x => x.SetValues(items[0], entity), Times.Once);
        context.Verify(x => x.SaveChangesAsync(token), Times.Once);
        dbSet.Verify(x => x.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        result.Created.Should().BeFalse();
    }
}