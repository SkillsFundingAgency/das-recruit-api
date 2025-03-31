using Microsoft.AspNetCore.Http.HttpResults;
using Recruit.Api.Application.Providers;
using Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Extensions;
using SFA.DAS.Recruit.Api.Models.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Tests.Controllers.ApplicationReviewControllerTests;

[TestFixture]
public class WhenGettingTheApplicationReview
{
    [Test, MoqAutoData]
    public async Task Get_ReturnsOk_WhenApplicationReviewExists(
        Guid id,
        ApplicationReviewEntity mockResponse,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        provider.Setup(p => p.GetById(id, token)).ReturnsAsync(mockResponse);

        // Act
        var result = await controller.Get(id, token);

        // Assert
        result.Should().BeOfType<Ok<GetApplicationReviewResponse>>();
        var okResult = result as Ok<GetApplicationReviewResponse>;
        okResult!.Value.Should().BeEquivalentTo(mockResponse.ToPutResponse());
    }

    [Test, MoqAutoData]
    public async Task Get_ReturnsNotFound_WhenApplicationReviewDoesNotExist(Guid id,
        ApplicationReviewEntity mockResponse,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        provider.Setup(p => p.GetById(id, token)).ReturnsAsync((ApplicationReviewEntity)null!);

        // Act
        var result = await controller.Get(id, token);

        // Assert
        result.Should().BeOfType<NotFound>();
    }

    [Test, MoqAutoData]
    public async Task Get_ReturnsInternalServerException_WhenException_Thrown(Guid id,
        ApplicationReviewEntity mockResponse,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> provider,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        provider.Setup(p => p.GetById(id, token)).ThrowsAsync(new Exception());

        // Act
        var result = await controller.Get(id, token);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}