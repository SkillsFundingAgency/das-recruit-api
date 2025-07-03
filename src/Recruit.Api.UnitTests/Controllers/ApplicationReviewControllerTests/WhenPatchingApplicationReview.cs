using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.Recruit.Api.Application.Providers;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.UnitTests.Controllers.ApplicationReviewControllerTests;

[TestFixture]
public class WhenPatchingApplicationReview
{
    [Test, MoqAutoData]
    public async Task Patch_Returns_Ok_When_Application_Review_Is_Patched(
        Guid id,
        JsonPatchDocument<ApplicationReview> patchDocument,
        ApplicationReviewEntity applicationReview,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
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
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        providerMock.Setup(p => p.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((ApplicationReviewEntity?)null);
        providerMock.Setup(p => p.GetByApplicationId(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((ApplicationReviewEntity?)null);

        // Act
        var result = await controller.Patch(id, patchDocument, CancellationToken.None);

        // Assert
        providerMock.Verify(p => p.GetById(id, CancellationToken.None), Times.Once);
        providerMock.Verify(p => p.Update(It.IsAny<ApplicationReviewEntity>(), CancellationToken.None), Times.Never);
        result.Should().BeOfType<NotFound>();
    }

    
    [Test, MoqAutoData]
    public async Task When_Application_Not_Found_By_Id_Found_By_ApplicationId_And_Updated(
        Guid id,
        JsonPatchDocument<ApplicationReview> patchDocument,
        ApplicationReviewEntity applicationReview,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        providerMock.Setup(p => p.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((ApplicationReviewEntity?)null);
        providerMock.Setup(p => p.GetByApplicationId(id, It.IsAny<CancellationToken>())).ReturnsAsync(applicationReview);
        providerMock.Setup(p => p.Update(It.Is<ApplicationReviewEntity>(c=>c.Id == applicationReview.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicationReview);

        // Act
        var result = await controller.Patch(id, patchDocument, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<Ok<PatchApplicationReviewResponse>>().Subject;
        var response = okResult.Value.Should().BeOfType<PatchApplicationReviewResponse>().Subject;
        response.Id.Should().Be(applicationReview.Id);
    }
    
    [Test, MoqAutoData]
    public async Task Patch_ReturnsInternalServerException_WhenApplicationReview_Throws_Exception(
        Guid id,
        JsonPatchDocument<ApplicationReview> patchDocument,
        ApplicationReviewEntity applicationReview,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller,
        CancellationToken token)
    {
        // Arrange
        providerMock.Setup(p => p.Update(It.IsAny<ApplicationReviewEntity>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        // Act
        var result = await controller.Patch(id, patchDocument, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}