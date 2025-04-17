using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Data.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.UnitTests.Application.Providers.ApplicationReviewsProviderTests;

[TestFixture]
internal class WhenGettingAllByUkprn
{
    [Test, MoqAutoData]
    public async Task GettingAllByUkprn_ShouldReturnPaginatedList_WhenCalledWithValidParameters(
        int ukprn,
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
        repositoryMock.Setup(repo => repo.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token))
            .ReturnsAsync(expectedList);

        // Act
        var result = await provider.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token);

        // Assert
        result.Should().BeEquivalentTo(expectedList);
        repositoryMock.Verify(repo => repo.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token), Times.Once);
    }

    [Test, MoqAutoData]
    public void GettingAllByUkprn_ShouldThrowException_WhenRepositoryThrowsException(
        int ukprn,
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

        repositoryMock.Setup(repo => repo.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token))
            .ThrowsAsync(new Exception("Repository exception"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(() => provider.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token));

        repositoryMock.Verify(repo => repo.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token), Times.Once);
    }
}