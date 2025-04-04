using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Tests.Controllers.ApplicationReviewControllerTests;

[TestFixture]
public class WhenPatchingApplicationReview
{
    [Test, MoqAutoData]
    public async Task Patch_Returns_Ok_When_Application_Review_Is_Patched(
        Guid id,
        JsonPatchDocument<ApplicationReview> patchDocument,
        ApplicationReviewEntity applicationReview,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        applicationReview.Id = id;
        providerMock.Setup(p => p.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(applicationReview);
        providerMock.Setup(p => p.Update(It.IsAny<ApplicationReviewEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(applicationReview);

        // Act
        var result = await controller.Patch(id, patchDocument, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<Ok<PatchApplicationReviewResponse>>().Subject;
        var response = okResult.Value.Should().BeOfType<PatchApplicationReviewResponse>().Subject;
        response.Id.Should().Be(id);
    }

    [Test, MoqAutoData]
    public async Task Patch_Returns_NotFound_When_Application_Review_Does_Not_Exist(
        Guid id,
        JsonPatchDocument<ApplicationReview> patchDocument,
        ApplicationReviewEntity applicationReview,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        providerMock.Setup(p => p.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((ApplicationReviewEntity?)null);

        // Act
        var result = await controller.Patch(id, patchDocument, CancellationToken.None);

        // Assert
        providerMock.Verify(p => p.GetById(id, CancellationToken.None), Times.Once);
        providerMock.Verify(p => p.Update(It.IsAny<ApplicationReviewEntity>(), CancellationToken.None), Times.Never);
        result.Should().BeOfType<NotFound>();
    }

    [Test, MoqAutoData]
    public async Task Patch_ReturnsInternalServerException_WhenApplicationReview_Throws_Exception(
        Guid id,
        JsonPatchDocument<ApplicationReview> patchDocument,
        ApplicationReviewEntity applicationReview,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        providerMock.Setup(p => p.Update(It.IsAny<ApplicationReviewEntity>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        // Act
        var result = await controller.Patch(id, patchDocument, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}