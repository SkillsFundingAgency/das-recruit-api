using Recruit.Api.Application.Providers;
using Recruit.Api.Data.ApplicationReview;
using Recruit.Api.Domain.Entities;
using Recruit.Api.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Application.Tests.Providers;

[TestFixture]
public class WhenGettingAllByAccountId
{
    [Test, MoqAutoData]
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
        var result = await provider.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

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
        Assert.ThrowsAsync<Exception>(() => provider.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token));

        repositoryMock.Verify(repo => repo.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token), Times.Once);
    }
}