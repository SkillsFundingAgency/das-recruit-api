using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using Recruit.Api.Application.Providers;
using Recruit.Api.Data.Models;
using Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Controllers;
using SFA.DAS.Recruit.Api.Models.Requests;
using SFA.DAS.Recruit.Api.Models.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace Recruit.Api.Tests.Controllers.ApplicationReviewControllerTests;

[TestFixture]
public class WhenPuttingApplicationReview
{
    [Test, MoqAutoData]
    public async Task Put_Returns_Created_When_New_Application_Review_Is_Created(Guid id,
        PutApplicationReviewRequest request,
        ApplicationReviewEntity applicationReview,
        Mock<IValidator<PutApplicationReviewRequest>> validator,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        ApplicationReviewEntity passedApplicationReview = null;
        applicationReview.Id = id;
        providerMock
            .Setup(p => p.Upsert(It.IsAny<ApplicationReviewEntity>(), It.IsAny<CancellationToken>()))
            .Callback((ApplicationReviewEntity entity, CancellationToken _) => passedApplicationReview = entity)
            .ReturnsAsync(UpsertResult.Create(applicationReview, true));
        validator
            .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await controller.Put(id, request, validator.Object, CancellationToken.None);
        var createdResult = result as Created<PutApplicationReviewResponse>;
            
        // Assert
        passedApplicationReview.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        createdResult.Should().NotBeNull();
        createdResult.Value.Should().BeEquivalentTo(applicationReview);
    }

    [Test, MoqAutoData]
    public async Task Put_Returns_Ok_When_Existing_Application_Review_Is_Updated(
        Guid id,
        PutApplicationReviewRequest request,
        ApplicationReviewEntity applicationReview,
        Mock<IValidator<PutApplicationReviewRequest>> validator,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        applicationReview.Id = id;
        providerMock.Setup(p => p.Upsert(It.IsAny<ApplicationReviewEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(UpsertResult.Create(applicationReview, false));
        validator
            .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await controller.Put(id, request, validator.Object, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<Ok<PutApplicationReviewResponse>>().Subject;
        var response = okResult.Value.Should().BeOfType<PutApplicationReviewResponse>().Subject;
        response.Id.Should().Be(id);
    }

    [Test, MoqAutoData]
    public async Task Put_ReturnsBadRequest_WhenExistingApplicationReviewIsUpdated(
        Guid id,
        PutApplicationReviewRequest request,
        ApplicationReviewEntity applicationReview,
        Mock<IValidator<PutApplicationReviewRequest>> validator,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        applicationReview.Id = id;
        validator
            .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult { Errors = [new ValidationFailure { PropertyName = "Foo", ErrorCode = "Foo" }] } );

        // Act
        var result = await controller.Put(id, request, validator.Object, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ValidationProblem>();
    }

    [Test, MoqAutoData]
    public async Task Put_ReturnsInternalException_WhenExistingApplicationReviewIsUpdated(
        Guid id,
        PutApplicationReviewRequest request,
        ApplicationReviewEntity applicationReview,
        Mock<IValidator<PutApplicationReviewRequest>> validator,
        [Frozen] Mock<IApplicationReviewsProvider> providerMock,
        [Greedy] ApplicationReviewController controller)
    {
        // Arrange
        applicationReview.Id = id;
        providerMock.Setup(p => p.Upsert(It.IsAny<ApplicationReviewEntity>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
        validator.Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        // Act
        var result = await controller.Put(id, request, validator.Object, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ProblemHttpResult>();
    }
}