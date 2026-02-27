using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.BlockedOrganisationRepositoryTests;

internal class WhenGettingBlockedOrganisation
{
    [Test, MoqAutoData]
    public async Task GetOneAsync_Returns_The_Correct_Item(
        Guid id,
        List<BlockedOrganisationEntity> items,
        BlockedOrganisationEntity expectedItem,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] BlockedOrganisationRepository sut)
    {
        // arrange
        items.Add(expectedItem);
        
        context.Setup(x => x.BlockedOrganisationEntities).ReturnsDbSet(items);

        // act
        var result = await sut.GetOneAsync(expectedItem.Id, CancellationToken.None);

        // assert
        result.Should().Be(expectedItem);
    }
    
    [Test, MoqAutoData]
    public async Task GetOneAsync_Returns_Null_When_Item_Not_Found(
        Guid id,
        List<BlockedOrganisationEntity> items,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] BlockedOrganisationRepository sut)
    {
        // arrange
        context.Setup(x => x.BlockedOrganisationEntities).ReturnsDbSet(items);

        // act
        var result = await sut.GetOneAsync(id, CancellationToken.None);

        // assert
        result.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task GetByOrganisationIdAsync_Returns_Items_In_Descending_Order(
        string organisationId,
        List<BlockedOrganisationEntity> items,
        BlockedOrganisationEntity expectedItem1,
        BlockedOrganisationEntity expectedItem2,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] BlockedOrganisationRepository sut)
    {
        // arrange
        var now = DateTime.UtcNow;
        expectedItem1.OrganisationId = organisationId;
        expectedItem1.UpdatedDate = now;
        expectedItem2.OrganisationId = organisationId;
        expectedItem2.UpdatedDate = now.AddDays(-2);
        
        items.AddRange([expectedItem1, expectedItem2]);
        
        context.Setup(x => x.BlockedOrganisationEntities).ReturnsDbSet(items);

        // act
        var result = await sut.GetByOrganisationIdAsync(organisationId, CancellationToken.None);

        // assert
        result.Should().HaveCount(2);
        result.Should().BeInDescendingOrder(x => x.UpdatedDate);
        result.Should().BeEquivalentTo([expectedItem1, expectedItem2]).And.BeInDescendingOrder(c=>c.UpdatedDate);
    }
}
