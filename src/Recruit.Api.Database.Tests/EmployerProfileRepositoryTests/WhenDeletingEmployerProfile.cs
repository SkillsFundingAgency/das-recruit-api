using Recruit.Api.Database.Tests.DatabaseMock;
using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.EmployerProfile;
using SFA.DAS.Recruit.Api.Domain.Entities;

namespace Recruit.Api.Database.Tests.EmployerProfileRepositoryTests;

public class WhenDeletingEmployerProfile
{
    [Test, MoqAutoData]
    public async Task DeleteOneAsync_Removes_Item_From_Context(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] EmployerProfileRepository sut,
        CancellationToken token)
    {
        // arrange
        var items = new List<EmployerProfileEntity> {
            new() { AccountLegalEntityId = 1, AccountId = 10 },
            new() { AccountLegalEntityId = 2, AccountId = 11 },
            new() { AccountLegalEntityId = 3, AccountId = 12 },
        };
        var dbSet = items.BuildDbSetMock();
        context.Setup(x => x.EmployerProfileEntities).Returns(dbSet.Object);

        // act
        bool result = await sut.DeleteOneAsync(2, token);

        // assert
        context.Verify(x => x.SaveChangesAsync(token), Times.Once);
        dbSet.Verify(x => x.Remove(items.Skip(1).First()), Times.Once);

        result.Should().BeTrue();
    }
    
    [Test, MoqAutoData]
    public async Task DeleteOneAsync_Returns_False_When_Not_Found(
        [Frozen] Mock<IRecruitDataContext> context,
        [Greedy] EmployerProfileRepository sut,
        CancellationToken token)
    {
        // arrange
        var items = new List<EmployerProfileEntity> {
            new() { AccountLegalEntityId = 1, AccountId = 10 },
        };
        var dbSet = items.BuildDbSetMock();
        context.Setup(x => x.EmployerProfileEntities).Returns(dbSet.Object);

        // act
        bool result = await sut.DeleteOneAsync(2, token);

        // assert
        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        dbSet.Verify(x => x.Remove(It.IsAny<EmployerProfileEntity>()), Times.Never);

        result.Should().BeFalse();
    }
}