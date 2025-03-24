using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using NUnit.Framework;
using Recruit.Api.Application.Models.ApplicationReview;
using Recruit.Api.Application.Providers;
using Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Models.Responses;
using SFA.DAS.Testing.AutoFixture;

[TestFixture]
public class WhenPatchingApplicationReview
{
    [Test, MoqAutoData]
    public async Task Patch_ReturnsOk_WhenApplicationReviewIsPatched(
        Guid id,
        JsonPatchDocument<PatchApplicationReview> patchDocument,
        ApplicationReviewEntity applicationReview,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        applicationReview.Id = id;
        providerMock.Setup(p => p.Update(It.IsAny<PatchApplication>(), It.IsAny<CancellationToken>())).ReturnsAsync(applicationReview);

        // Act
        var result = await controller.Patch(id, patchDocument);

        // Assert
        var okResult = result.Should().BeOfType<Ok<ApplicationReviewResponse>>().Subject;
        var response = okResult.Value.Should().BeOfType<ApplicationReviewResponse>().Subject;
        response.Id.Should().Be(id);
    }

    [Test, MoqAutoData]
    public async Task Patch_ReturnsNotFound_WhenApplicationReviewDoesNotExist(
        Guid id,
        JsonPatchDocument<PatchApplicationReview> patchDocument,
        ApplicationReviewEntity applicationReview,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        providerMock.Setup(p => p.Update(It.IsAny<PatchApplication>(), It.IsAny<CancellationToken>())).ReturnsAsync((ApplicationReviewEntity)null!);

        // Act
        var result = await controller.Patch(id, patchDocument);

        // Assert
        result.Should().BeOfType<NotFound>();
    }

    [Test, MoqAutoData]
    public async Task Patch_ReturnsInternalServerException_WhenApplicationReview_Throws_Exception(
        Guid id,
        JsonPatchDocument<PatchApplicationReview> patchDocument,
        ApplicationReviewEntity applicationReview,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        providerMock.Setup(p => p.Update(It.IsAny<PatchApplication>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        // Act
        var result = await controller.Patch(id, patchDocument);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}
