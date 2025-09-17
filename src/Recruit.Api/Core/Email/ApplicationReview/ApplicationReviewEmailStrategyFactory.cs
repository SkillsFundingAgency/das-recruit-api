using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using NotSupportedException = SFA.DAS.Recruit.Api.Core.Exceptions.NotSupportedException;

namespace SFA.DAS.Recruit.Api.Core.Email.ApplicationReview;

public interface IApplicationReviewEmailStrategyFactory
{
    IApplicationReviewEmailStrategy Create(ApplicationReviewEntity applicationReview);
}

internal class ApplicationReviewEmailStrategyFactory(
    SharedApplicationEmailStrategy sharedApplicationEmailStrategy,
    EmployerHasReviewedApplicationEmailStrategy employerHasReviewedApplicationEmailStrategy,
    NewApplicationEmailStrategy newApplicationEmailStrategy
    ) : IApplicationReviewEmailStrategyFactory
{
    public IApplicationReviewEmailStrategy Create(ApplicationReviewEntity applicationReview)
    {
        ArgumentNullException.ThrowIfNull(applicationReview, nameof(applicationReview));
        
        return applicationReview.Status switch {
            ApplicationReviewStatus.EmployerInterviewing => employerHasReviewedApplicationEmailStrategy,
            ApplicationReviewStatus.EmployerUnsuccessful => employerHasReviewedApplicationEmailStrategy,
            ApplicationReviewStatus.New => newApplicationEmailStrategy,
            ApplicationReviewStatus.Shared => sharedApplicationEmailStrategy,
            _ => throw new NotSupportedException($"Missing email handler: no registered handler for Application Review Status {applicationReview.Status}")
        };
    }
}