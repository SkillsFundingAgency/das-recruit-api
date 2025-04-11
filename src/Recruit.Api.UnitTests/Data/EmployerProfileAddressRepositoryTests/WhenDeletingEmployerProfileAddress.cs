using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.EmployerProfileAddressRepositoryTests;

internal class WhenDeletingEmployerProfileAddress
{
    [Test, MoqAutoData]
    public async Task DeleteOneAsync_Removes_Item_From_Context(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] EmployerProfileAddressRepository sut,
        CancellationToken token)
    {
        // arrange
        List<EmployerProfileAddressEntity> items = [
            new() { Id = 1, AccountLegalEntityId = 1, AddressLine1 = "required field", Postcode = "required field" },
        ];
        var dbSet = items.BuildDbSetMock();
        context.Setup(x => x.EmployerProfileAddressEntities).Returns(dbSet.Object);

        // act
        bool result = await sut.DeleteOneAsync(new EmployerProfileAddressKey(1, 1), token);

        // assert
        context.Verify(x => x.SaveChangesAsync(token), Times.Once);
        dbSet.Verify(x => x.Remove(items[0]), Times.Once);

        result.Should().BeTrue();
    }
    
    [Test, MoqAutoData]
    public async Task DeleteOneAsync_Returns_False_When_Not_Found(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] EmployerProfileAddressRepository sut,
        CancellationToken token)
    {
        // arrange
        List<EmployerProfileAddressEntity> items = [
            new() { Id = 1, AccountLegalEntityId = 1, AddressLine1 = "required field", Postcode = "required field" },
        ];
        var dbSet = items.BuildDbSetMock();
        context.Setup(x => x.EmployerProfileAddressEntities).Returns(dbSet.Object);

        // act
        bool result = await sut.DeleteOneAsync(new EmployerProfileAddressKey(2, 2), token);

        // assert
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        dbSet.Verify(x => x.Remove(It.IsAny<EmployerProfileAddressEntity>()), Times.Never);

        result.Should().BeFalse();
    }
}