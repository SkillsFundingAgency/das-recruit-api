using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using Recruit.Api.Application.Providers;
using Recruit.Api.Domain.Entities;
using Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Tests.Controllers.ApplicationReviewControllerTests;

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
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        var pagedResult = new PaginatedList<ApplicationReviewEntity>(mockResponse.ToList(), 1, pageNumber, pageSize);
        providerMock.Setup(p => p.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token))
                    .ReturnsAsync(pagedResult);

        // Act
        var result = await controller.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

        // Assert
        result.Should().BeOfType<Ok<ApplicationReviewController.ApplicationReviewsResponse>>();
        var okResult = result as Ok<ApplicationReviewController.ApplicationReviewsResponse>;

        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value!.ApplicationReviews.Count().Should().BeGreaterThan(1);
    }

    [Test, MoqAutoData]
    public async Task GetAllByAccountId_Returns_Empty_WhenNoApplicationReviewsExist(
        long accountId,
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
        providerMock.Setup(p => p.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token))
                    .ReturnsAsync(pagedResult);

        // Act
        var result = await controller.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

        // Assert
        result.Should().BeOfType<Ok<ApplicationReviewController.ApplicationReviewsResponse>>();
        var okResult = result as Ok<ApplicationReviewController.ApplicationReviewsResponse>;

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
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        // Arrange
        providerMock.Setup(p => p.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token))
            .ThrowsAsync(new Exception());

        // Act
        var result = await controller.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }

}