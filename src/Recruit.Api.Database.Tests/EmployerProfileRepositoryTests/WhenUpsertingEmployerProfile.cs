using Recruit.Api.Database.Tests.DatabaseMock;
using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace Recruit.Api.Database.Tests.EmployerProfileRepositoryTests;

public class WhenUpsertingEmployerProfile
{
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }
    
    [Test, MoqAutoData]
    public async Task UpsertOneAsync_Inserts_New_Entity(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] EmployerProfileRepository sut,
        CancellationToken token)
    {
        // arrange
        var entity = _fixture.Create<EmployerProfileEntity>();
        var dbSet = new List<EmployerProfileEntity>().BuildDbSetMock();
        context.Setup(x => x.EmployerProfileEntities).Returns(dbSet.Object);

        // act
        var result = await sut.UpsertOneAsync(entity, token);

        // assert
        context.Verify(x => x.SaveChangesAsync(token), Times.Once);
        dbSet.Verify(x => x.AddAsync(entity, token), Times.Once);

        result.Created.Should().BeTrue();
    }
    
    [Test, MoqAutoData]
    public async Task UpsertOneAsync_Updates_Existing_Entity(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] EmployerProfileRepository sut,
        CancellationToken token)
    {
        // arrange
        var entity = new EmployerProfileEntity { AccountLegalEntityId = 1, AccountId = 10, TradingName = "Test" };
        List<EmployerProfileEntity> items = [
            new() { AccountLegalEntityId = 1, AccountId = 10 },
        ];
        var dbSet = items.BuildDbSetMock();
        context.Setup(x => x.EmployerProfileEntities).Returns(dbSet.Object);

        // act
        var result = await sut.UpsertOneAsync(entity, token);

        // assert
        context.Verify(x => x.SetValues(items[0], entity), Times.Once);
        context.Verify(x => x.SaveChangesAsync(token), Times.Once);
        dbSet.Verify(x => x.AddAsync(It.IsAny<EmployerProfileEntity>(), It.IsAny<CancellationToken>()), Times.Never);

        result.Created.Should().BeFalse();
    }
}