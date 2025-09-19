using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Data.Providers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.EmployerAccountControllerTests;

[TestFixture]
public class WhenGettingAllByAccountId
{
    [Test, MoqAutoData]
    public async Task GetAllByAccountId_ReturnsOk_WhenApplicationReviewsExist(
        long accountId,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        List<ApplicationReviewEntity> mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] EmployerAccountController controller,
        CancellationToken token)
    {
        // Arrange
        var pagedResult = new PaginatedList<ApplicationReviewEntity>(mockResponse.ToList(), 1, pageNumber, pageSize);
        providerMock.Setup(p => p.GetPagedAccountIdAsync(accountId, pageNumber, pageSize, sortColumn, isAscending, token))
                    .ReturnsAsync(pagedResult);

        // Act
        var result = await controller.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

        // Assert
        result.Should().BeOfType<Ok<ApplicationReviewsResponse>>();
        var okResult = result as Ok<ApplicationReviewsResponse>;

        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value!.ApplicationReviews.Count().Should().Be(mockResponse.Count);
    }

    [Test, MoqAutoData]
    public async Task GetAllByAccountId_Returns_Empty_WhenNoApplicationReviewsExist(
        long accountId,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        List<ApplicationReviewEntity> mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] EmployerAccountController controller,
        CancellationToken token)
    {
        // Arrange
        var pagedResult = new PaginatedList<ApplicationReviewEntity>([], 0, pageNumber, pageSize);
        providerMock.Setup(p => p.GetPagedAccountIdAsync(accountId, pageNumber, pageSize, sortColumn, isAscending, token))
                    .ReturnsAsync(pagedResult);

        // Act
        var result = await controller.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

        // Assert
        result.Should().BeOfType<Ok<ApplicationReviewsResponse>>();
        var okResult = result as Ok<ApplicationReviewsResponse>;

        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value!.ApplicationReviews.Count().Should().Be(0);
    }

    [Test, MoqAutoData]
    public async Task GetAllByAccountId_Returns_Exception_WhenInvalidAccountId(
        long accountId,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] EmployerAccountController controller,
        CancellationToken token)
    {
        // Arrange
        // Arrange
        providerMock.Setup(p => p.GetPagedAccountIdAsync(accountId, pageNumber, pageSize, sortColumn, isAscending, token))
            .ThrowsAsync(new Exception());

        // Act
        var result = await controller.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}