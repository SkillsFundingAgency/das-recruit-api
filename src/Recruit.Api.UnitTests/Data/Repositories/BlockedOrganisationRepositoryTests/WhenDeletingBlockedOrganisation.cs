using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.BlockedOrganisationRepositoryTests;

internal class WhenDeletingBlockedOrganisation
{
    [Test, MoqAutoData]
    public async Task DeleteOneAsync_Returns_False_When_Item_Not_Found(
        Guid id,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] BlockedOrganisationRepository sut)
    {
        // arrange
        context.Setup(x => x.BlockedOrganisationEntities).ReturnsDbSet((List<BlockedOrganisationEntity>)[]);

        // act
        var result = await sut.DeleteOneAsync(id, CancellationToken.None);

        // assert
        result.Should().BeFalse();
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task DeleteOneAsync_Returns_True_When_Item_Deleted(
        BlockedOrganisationEntity entity,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] BlockedOrganisationRepository sut,
        CancellationToken token)
    {
        // arrange
        List<BlockedOrganisationEntity> items = [entity];
        var dbSet = items.BuildDbSetMock();
        context.Setup(x => x.BlockedOrganisationEntities).Returns(dbSet.Object);

        // act
        var result = await sut.DeleteOneAsync(entity.Id, token);

        // assert
        result.Should().BeTrue();
        dbSet.Verify(x => x.Remove(entity), Times.Once);
        context.Verify(x => x.SaveChangesAsync(token), Times.Once);
    }
}
