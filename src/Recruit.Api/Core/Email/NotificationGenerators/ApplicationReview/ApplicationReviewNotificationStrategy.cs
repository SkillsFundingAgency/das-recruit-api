using SFA.DAS.Recruit.Api.Core.Exceptions;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.ApplicationReview;

public interface IApplicationReviewNotificationStrategy
{
    IApplicationReviewNotificationFactory Create(ApplicationReviewEntity applicationReview);
}

public class ApplicationReviewNotificationStrategy(
    ApplicationSharedWithEmployerNotificationFactory applicationSharedWithEmployerNotificationFactory,
    SharedApplicationReviewedByEmployerNotificationFactory sharedApplicationReviewedByEmployerNotificationFactory,
    ApplicationSubmittedNotificationFactory applicationSubmittedNotificationFactory
    ) : IApplicationReviewNotificationStrategy
{
    public IApplicationReviewNotificationFactory Create(ApplicationReviewEntity applicationReview)
    {
        ArgumentNullException.ThrowIfNull(applicationReview);
        
        return applicationReview.Status switch {
            ApplicationReviewStatus.EmployerInterviewing => sharedApplicationReviewedByEmployerNotificationFactory,
            ApplicationReviewStatus.EmployerUnsuccessful => sharedApplicationReviewedByEmployerNotificationFactory,
            ApplicationReviewStatus.New => applicationSubmittedNotificationFactory,
            ApplicationReviewStatus.Shared => applicationSharedWithEmployerNotificationFactory,
            _ => throw new EntityStateNotSupportedException($"Missing email handler: no registered handler for Application Review Status {applicationReview.Status}")
        };
    }
}