using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Recruit.Api.Application.Providers;
using Recruit.Api.Domain.Entities;
using Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Tests.Controllers.ApplicationReviewControllerTests;

[TestFixture]
public class WhenGettingAllByUkprn
{
    [Test, MoqAutoData]
    public async Task GetAllByUkprn_ReturnsOk_WhenApplicationReviewsExist(
        int ukprn,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        List<ApplicationReviewEntity> mockResponse,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        var pagedResult = new PaginatedList<ApplicationReviewEntity>(mockResponse.ToList(), 1, pageNumber, pageSize);
        providerMock.Setup(p => p.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token))
                    .ReturnsAsync(pagedResult);

        // Act
        var result = await controller.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token);

        // Assert
        result.Should().BeOfType<Ok<ApplicationReviewController.ApplicationReviewsResponse>>();
        var okResult = result as Ok<ApplicationReviewController.ApplicationReviewsResponse>;

        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value!.ApplicationReviews.Count().Should().Be(mockResponse.Count);
    }

    [Test, MoqAutoData]
    public async Task GetAllByUkprn_Returns_Empty_WhenNoApplicationReviewsExist(
        int ukprn,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        List<ApplicationReviewEntity> mockResponse,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        var pagedResult = new PaginatedList<ApplicationReviewEntity>([], 0, pageNumber, pageSize);
        providerMock.Setup(p => p.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token))
                    .ReturnsAsync(pagedResult);

        // Act
        var result = await controller.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token);

        // Assert
        result.Should().BeOfType<Ok<ApplicationReviewController.ApplicationReviewsResponse>>();
        var okResult = result as Ok<ApplicationReviewController.ApplicationReviewsResponse>;

        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value!.ApplicationReviews.Count().Should().Be(0);
    }

    [Test, MoqAutoData]
    public async Task GetAllByAccountId_Returns_Exception_WhenInvalidAccountId(
        int ukprn,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        // Arrange
        providerMock.Setup(p => p.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token))
            .ThrowsAsync(new Exception());

        // Act
        var result = await controller.GetAllByUkprn(ukprn, pageNumber, pageSize, sortColumn, isAscending, token);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }

}