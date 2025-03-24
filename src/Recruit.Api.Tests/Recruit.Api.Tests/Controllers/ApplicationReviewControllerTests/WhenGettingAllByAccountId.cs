using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Recruit.Api.Application.Providers;
using Recruit.Api.Domain.Entities;
using Recruit.Api.Domain.Enums;
using Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Extensions;
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
    CancellationToken token,
    [Frozen] Mock<IApplicationReviewsProvider> providerMock,
    [Greedy] ApplicationReviewController controller)
{
    // Arrange
    var applicationReviews = new List<ApplicationReview> { new ApplicationReview() };
    var pagedResult = new PaginatedList<ApplicationReviewEntity>(applicationReviews, 1, pageNumber, pageSize);
    providerMock.Setup(p => p.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token))
                .ReturnsAsync(pagedResult);

    // Act
    var result = await controller.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

    // Assert
    result.Should().BeOfType<OkObjectResult>();
    var okResult = result as OkObjectResult;
    var response = okResult.Value as ApplicationReviewsResponse;
    response.ApplicationReviews.Should().HaveCount(1);
}

[Test, MoqAutoData]
public async Task GetAllByAccountId_ReturnsNotFound_WhenNoApplicationReviewsExist(
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
    var pagedResult = new PagedResult<ApplicationReview>(new List<ApplicationReview>(), 0, pageNumber, pageSize);
    providerMock.Setup(p => p.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token))
                .ReturnsAsync(pagedResult);

    // Act
    var result = await controller.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

    // Assert
    result.Should().BeOfType<OkObjectResult>();
    var okResult = result as OkObjectResult;
    var response = okResult.Value as ApplicationReviewsResponse;
    response.ApplicationReviews.Should().BeEmpty();
}

[Test, MoqAutoData]
public async Task GetAllByAccountId_ReturnsBadRequest_WhenInvalidAccountId(
    long accountId,
    int pageNumber,
    int pageSize,
    string sortColumn,
    bool isAscending,
    CancellationToken token,
    [Greedy] ApplicationReviewController controller)
{
    // Act
    var result = await controller.GetAllByAccountId(accountId, pageNumber, pageSize, sortColumn, isAscending, token);

    // Assert
    result.Should().BeOfType<BadRequestResult>();
}

}