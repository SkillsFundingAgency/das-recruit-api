using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.BlockedOrganisationRepositoryTests;

internal class WhenUpsertingBlockedOrganisation
{
    [Test, RecursiveMoqAutoData]
    public async Task UpsertOneAsync_Inserts_New_Entity(
        BlockedOrganisationEntity entity,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] BlockedOrganisationRepository sut,
        CancellationToken token)
    {
        // arrange
        var dbSet = new List<BlockedOrganisationEntity>().BuildDbSetMock();
        context.Setup(x => x.BlockedOrganisationEntities).Returns(dbSet.Object);

        // act
        var result = await sut.UpsertOneAsync(entity, token);

        // assert
        context.Verify(x => x.SaveChangesAsync(token), Times.Once);
        dbSet.Verify(x => x.AddAsync(entity, token), Times.Once);

        result.Created.Should().BeTrue();
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task UpsertOneAsync_Updates_Existing_Entity(
        BlockedOrganisationEntity existingEntity,
        BlockedOrganisationEntity updatedEntity,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] BlockedOrganisationRepository sut,
        CancellationToken token)
    {
        // arrange
        updatedEntity.Id = existingEntity.Id;
        List<BlockedOrganisationEntity> items = [existingEntity];
        var dbSet = items.BuildDbSetMock();
        context.Setup(x => x.BlockedOrganisationEntities).Returns(dbSet.Object);

        // act
        var result = await sut.UpsertOneAsync(updatedEntity, token);

        // assert
        context.Verify(x => x.SetValues(existingEntity, updatedEntity), Times.Once);
        context.Verify(x => x.SaveChangesAsync(token), Times.Once);
        dbSet.Verify(x => x.AddAsync(It.IsAny<BlockedOrganisationEntity>(), It.IsAny<CancellationToken>()), Times.Never);

        result.Created.Should().BeFalse();
    }
}
