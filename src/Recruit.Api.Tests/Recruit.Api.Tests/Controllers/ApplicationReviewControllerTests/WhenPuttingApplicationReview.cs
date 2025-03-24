using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using Recruit.Api.Application.Providers;
using Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Models.Requests;
using SFA.DAS.Recruit.Api.Models.Responses;
using SFA.DAS.Testing.AutoFixture;

[TestFixture]
public class WhenPuttingApplicationReview
{

    [Test, MoqAutoData]
    public async Task Put_ReturnsCreated_WhenNewApplicationReviewIsCreated(Guid id,
        ApplicationReviewRequest request,
        ApplicationReviewEntity applicationReview,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        applicationReview.Id = id;
        providerMock.Setup(p => p.Upsert(It.IsAny<ApplicationReviewEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(Tuple.Create(applicationReview, true));

        // Act
        var result = await controller.Put(id, request, token);

        // Assert
        var createdResult = result.Should().BeOfType<Created<ApplicationReviewResponse>>().Subject;
        var response = createdResult.Should().BeOfType<Created<ApplicationReviewResponse>>().Subject;
        response.Value!.Id.Should().Be(id);
    }

    [Test, MoqAutoData]
    public async Task Put_ReturnsOk_WhenExistingApplicationReviewIsUpdated(
        Guid id,
        ApplicationReviewRequest request,
        ApplicationReviewEntity applicationReview,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        applicationReview.Id = id;
        providerMock.Setup(p => p.Upsert(It.IsAny<ApplicationReviewEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(Tuple.Create(applicationReview, false));

        // Act
        var result = await controller.Put(id, request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<Ok<ApplicationReviewResponse>>().Subject;
        var response = okResult.Value.Should().BeOfType<ApplicationReviewResponse>().Subject;
        response.Id.Should().Be(id);
    }

    [Test, MoqAutoData]
    public async Task Put_ReturnsBadRequest_WhenExistingApplicationReviewIsUpdated(
        Guid id,
        ApplicationReviewRequest request,
        ApplicationReviewEntity applicationReview,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        applicationReview.Id = id;
        providerMock.Setup(p => p.Upsert(It.IsAny<ApplicationReviewEntity>(), It.IsAny<CancellationToken>())).ThrowsAsync(new ValidationException(""));

        // Act
        var result = await controller.Put(id, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }

    [Test, MoqAutoData]
    public async Task Put_ReturnsInternalException_WhenExistingApplicationReviewIsUpdated(
        Guid id,
        ApplicationReviewRequest request,
        ApplicationReviewEntity applicationReview,
        CancellationToken token,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        applicationReview.Id = id;
        providerMock.Setup(p => p.Upsert(It.IsAny<ApplicationReviewEntity>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        // Act
        var result = await controller.Put(id, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}