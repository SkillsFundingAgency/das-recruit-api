using SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.ApplicationReview;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Core.Email.NotificationGenerators.ApplicationReview;

internal class WhenGettingFactory
{
    [Test, RecursiveMoqAutoData]
    public void Then_The_Application_Submitted_Factory_Is_Returned_For_New_Application_Reviews(
        ApplicationReviewEntity applicationReview,
        [Frozen] ApplicationSubmittedNotificationFactory expectedFactory,
        [Greedy] ApplicationReviewNotificationStrategy sut)
    {
        // arrange
        applicationReview.Status = ApplicationReviewStatus.New;

        // act
        var result = sut.Create(applicationReview);

        // assert
        result.Should().Be(expectedFactory);
    }
    
    [Test, RecursiveMoqAutoData]
    public void Then_The_Application_Shared_With_Employer_Factory_Is_Returned_For_Shared_Application_Reviews(
        ApplicationReviewEntity applicationReview,
        [Frozen] ApplicationSharedWithEmployerNotificationFactory expectedFactory,
        [Greedy] ApplicationReviewNotificationStrategy sut)
    {
        // arrange
        applicationReview.Status = ApplicationReviewStatus.Shared;

        // act
        var result = sut.Create(applicationReview);

        // assert
        result.Should().Be(expectedFactory);
    }
    
    [Test]
    [RecursiveMoqInlineAutoData(ApplicationReviewStatus.EmployerUnsuccessful)]
    [RecursiveMoqInlineAutoData(ApplicationReviewStatus.EmployerInterviewing)]
    public void Then_The_Shared_Application_Revewied_By_Employer_Factory_Is_Returned_For_Reviewed_Application_Reviews(
        ApplicationReviewStatus status,
        ApplicationReviewEntity applicationReview,
        [Frozen] SharedApplicationReviewedByEmployerNotificationFactory expectedFactory,
        [Greedy] ApplicationReviewNotificationStrategy sut)
    {
        // arrange
        applicationReview.Status = status;

        // act
        var result = sut.Create(applicationReview);

        // assert
        result.Should().Be(expectedFactory);
    }
}