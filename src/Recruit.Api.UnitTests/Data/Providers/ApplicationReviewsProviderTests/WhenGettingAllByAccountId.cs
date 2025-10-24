using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Providers.ApplicationReviewsProviderTests;

[TestFixture]
internal class WhenGettingAllByAccountId
{
    [Test, RecursiveMoqAutoData]
    public async Task GetAllByAccountId_ShouldReturnPaginatedList_WhenCalledWithValidParameters(
        long accountId,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        CancellationToken token,
        List<ApplicationReviewEntity> entities,
        [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
        [Greedy] ApplicationReviewsProvider provider)
    {
        // Arrange
        sortColumn = "CreatedDate";
        isAscending = false;

        var expectedList = new PaginatedList<ApplicationReviewEntity>(entities, 10, pageNumber, pageSize);
        repositoryMock.Setup(repo => repo.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token))
            .ReturnsAsync(expectedList);

        // Act
        var result = await provider.GetPagedAccountIdAsync(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

        // Assert
        result.Should().BeEquivalentTo(expectedList);
        repositoryMock.Verify(repo => repo.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token), Times.Once);
    }

    [Test, MoqAutoData]
    public void GetAllByAccountId_ShouldThrowException_WhenRepositoryThrowsException(
        long accountId,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewRepository> repositoryMock,
        [Greedy] ApplicationReviewsProvider provider)
    {
        // Arrange
        sortColumn = "CreatedDate";
        isAscending = false;

        repositoryMock.Setup(repo => repo.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token))
            .ThrowsAsync(new Exception("Repository exception"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(() => provider.GetPagedAccountIdAsync(accountId, pageNumber, pageSize, sortColumn, isAscending, token));

        repositoryMock.Verify(repo => repo.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token), Times.Once);
    }
}