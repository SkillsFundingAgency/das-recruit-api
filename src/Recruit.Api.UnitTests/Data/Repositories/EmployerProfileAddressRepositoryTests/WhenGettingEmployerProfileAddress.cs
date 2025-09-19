using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.EmployerProfileAddressRepositoryTests;

internal class WhenGettingEmployerProfileAddress
{
    [Test, MoqAutoData]
    public async Task GetOneAsync_Returns_The_Correct_Item(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] EmployerProfileAddressRepository sut)
    {
        // arrange
        List<EmployerProfileAddressEntity> items = [
            new() { Id = 1, AccountLegalEntityId = 1, AddressLine1 = "required field", Postcode = "required field" },
            new() { Id = 2, AccountLegalEntityId = 1, AddressLine1 = "required field", Postcode = "required field" },
            new() { Id = 3, AccountLegalEntityId = 1, AddressLine1 = "required field", Postcode = "required field" },
        ];
        
        context.Setup(x => x.EmployerProfileAddressEntities).ReturnsDbSet(items);

        // act
        var result = await sut.GetOneAsync(new EmployerProfileAddressKey(1, 2), CancellationToken.None);

        // assert
        result.Should().Be(items[1]);
    }
    
    [Test, MoqAutoData]
    public async Task GetOneAsync_Returns_Null_When_Item_Not_Found(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] EmployerProfileAddressRepository sut)
    {
        // arrange
        List<EmployerProfileAddressEntity> items = [
            new() { Id = 1, AccountLegalEntityId = 1, AddressLine1 = "required field", Postcode = "required field" },
        ];
        
        context.Setup(x => x.EmployerProfileAddressEntities).ReturnsDbSet(items);

        // act
        var result = await sut.GetOneAsync(new EmployerProfileAddressKey(1, 2), CancellationToken.None);

        // assert
        result.Should().BeNull();
    }
    
    [Test, MoqAutoData]
    public async Task GetManyAsync_Returns_The_Correct_Item(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] EmployerProfileAddressRepository sut)
    {
        // arrange
        List<EmployerProfileAddressEntity> items = [
            new() { Id = 1, AccountLegalEntityId = 1, AddressLine1 = "required field", Postcode = "required field" },
            new() { Id = 2, AccountLegalEntityId = 1, AddressLine1 = "required field", Postcode = "required field" },
            new() { Id = 3, AccountLegalEntityId = 1, AddressLine1 = "required field", Postcode = "required field" },
            new() { Id = 1, AccountLegalEntityId = 2, AddressLine1 = "required field", Postcode = "required field" },
        ];
        
        context.Setup(x => x.EmployerProfileAddressEntities).ReturnsDbSet(items);

        // act
        var result = await sut.GetManyAsync(1, CancellationToken.None);

        // assert
        result.Should().BeEquivalentTo(items.Take(3));
    }
    
    [Test, MoqAutoData]
    public async Task GetManyAsync_Returns_Empty_Array_For_No_Records(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] EmployerProfileAddressRepository sut)
    {
        // arrange
        List<EmployerProfileAddressEntity> items = [
            new() { Id = 1, AccountLegalEntityId = 2, AddressLine1 = "required field", Postcode = "required field" },
        ];
        
        context.Setup(x => x.EmployerProfileAddressEntities).ReturnsDbSet(items);

        // act
        var result = await sut.GetManyAsync(1, CancellationToken.None);

        // assert
        result.Should().BeEmpty();
    }
}