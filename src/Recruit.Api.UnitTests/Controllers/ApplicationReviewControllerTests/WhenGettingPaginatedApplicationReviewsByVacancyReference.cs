using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ApplicationReviewControllerTests;
[TestFixture]
internal class WhenGettingPaginatedApplicationReviewsByVacancyReference
{
    [Test, MoqAutoData]
    public async Task GetAllByVacancyReference_ReturnsOk_WhenApplicationReviewsExist(
        long vacancyReference,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        List<ApplicationReviewEntity> mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        var pagedResult = new PaginatedList<ApplicationReviewEntity>(mockResponse.ToList(), 1, pageNumber, pageSize);
        providerMock.Setup(p => p.GetPagedByVacancyReferenceAsync(vacancyReference, pageNumber, pageSize, sortColumn, isAscending, token))
                    .ReturnsAsync(pagedResult);

        // Act
        var result = await controller.GetPagedByVacancyReference(vacancyReference, pageNumber, pageSize, sortColumn, isAscending, token);

        // Assert
        result.Should().BeOfType<Ok<GetPagedApplicationReviewsResponse>>();
        var okResult = result as Ok<GetPagedApplicationReviewsResponse>;

        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value!.Items.Count().Should().Be(mockResponse.Count);
    }

    [Test, MoqAutoData]
    public async Task GetAllByVacancyReference_Returns_Empty_WhenNoApplicationReviewsExist(
        long vacancyReference,
        int pageNumber,
        int pageSize,
        string sortColumn,
        bool isAscending,
        List<ApplicationReviewEntity> mockResponse,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        var pagedResult = new PaginatedList<ApplicationReviewEntity>([], 0, pageNumber, pageSize);
        providerMock.Setup(p => p.GetPagedByVacancyReferenceAsync(vacancyReference, pageNumber, pageSize, sortColumn, isAscending, token))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await controller.GetPagedByVacancyReference(vacancyReference, pageNumber, pageSize, sortColumn, isAscending, token);

        // Assert
        result.Should().BeOfType<Ok<GetPagedApplicationReviewsResponse>>();
        var okResult = result as Ok<GetPagedApplicationReviewsResponse>;

        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value!.Items.Count().Should().Be(0);
        okResult.Value!.Info.TotalCount.Should().Be(0);
        okResult.Value!.Info.PageIndex.Should().Be(pageNumber);
        okResult.Value!.Info.PageSize.Should().Be(pageSize);
        okResult.Value!.Info.TotalPages.Should().Be(0);
    }
}
