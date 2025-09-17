using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests.Data.DatabaseMock;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.EmployerProfileRepositoryTests;

internal class WhenGettingEmployerProfile
{
    [Test, MoqAutoData]
    public async Task GetOneAsync_Returns_The_Correct_Item(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] EmployerProfileRepository sut)
    {
        // arrange
        List<EmployerProfileEntity> items = [
            new() { AccountLegalEntityId = 1, AccountId = 10 },
            new() { AccountLegalEntityId = 2, AccountId = 11 },
            new() { AccountLegalEntityId = 3, AccountId = 12 },
        ];
        
        context.Setup(x => x.EmployerProfileEntities).ReturnsDbSet(items);

        // act
        var result = await sut.GetOneAsync(2, CancellationToken.None);

        // assert
        result.Should().Be(items[1]);
    }
    
    [Test, MoqAutoData]
    public async Task GetOneAsync_Returns_Null_When_Item_Not_Found(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] EmployerProfileRepository sut)
    {
        // arrange
        List<EmployerProfileEntity> items = [
            new() { AccountLegalEntityId = 1, AccountId = 10 },
        ];
        
        context.Setup(x => x.EmployerProfileEntities).ReturnsDbSet(items);

        // act
        var result = await sut.GetOneAsync(2, CancellationToken.None);

        // assert
        result.Should().BeNull();
    }
}