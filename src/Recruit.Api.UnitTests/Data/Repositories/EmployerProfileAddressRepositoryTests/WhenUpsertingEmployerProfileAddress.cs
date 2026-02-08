using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Testing.Data;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.EmployerProfileAddressRepositoryTests;

internal class WhenUpsertingEmployerProfileAddress
{
    [Test, RecursiveMoqAutoData]
    public async Task UpsertOneAsync_Inserts_New_Entity(
        EmployerProfileAddressEntity entity,
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] EmployerProfileAddressRepository sut,
        CancellationToken token)
    {
        // arrange
        var dbSet = new List<EmployerProfileAddressEntity>().BuildDbSetMock();
        context.Setup(x => x.EmployerProfileAddressEntities).Returns(dbSet.Object);

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
        [Greedy] EmployerProfileAddressRepository sut,
        CancellationToken token)
    {
        // arrange
        var entity = new EmployerProfileAddressEntity { Id = 1, AccountLegalEntityId = 1, AddressLine1 = "new required field", Postcode = "new required field" };
        List<EmployerProfileAddressEntity> items = [
            new() { Id = 1, AccountLegalEntityId = 1, AddressLine1 = "old required field", Postcode = "old required field" },
        ];
        var dbSet = items.BuildDbSetMock();
        context.Setup(x => x.EmployerProfileAddressEntities).Returns(dbSet.Object);

        // act
        var result = await sut.UpsertOneAsync(entity, token);

        // assert
        context.Verify(x => x.SetValues(items[0], entity), Times.Once);
        context.Verify(x => x.SaveChangesAsync(token), Times.Once);
        dbSet.Verify(x => x.AddAsync(It.IsAny<EmployerProfileAddressEntity>(), It.IsAny<CancellationToken>()), Times.Never);

        result.Created.Should().BeFalse();
    }
}