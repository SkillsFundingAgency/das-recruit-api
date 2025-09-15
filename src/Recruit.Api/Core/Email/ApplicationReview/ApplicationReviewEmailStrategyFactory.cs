using SFA.DAS.Recruit.Api.Core.Exceptions;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Core.Email.ApplicationReview;

public interface IApplicationReviewEmailStrategyFactory
{
    IApplicationReviewEmailStrategy Create(ApplicationReviewEntity applicationReview);
}

internal class ApplicationReviewEmailStrategyFactory(
    SharedApplicationEmailStrategy sharedApplicationEmailStrategy,
    EmployerHasReviewedApplicationEmailStrategy employerHasReviewedApplicationEmailStrategy
    ) : IApplicationReviewEmailStrategyFactory
{
    public IApplicationReviewEmailStrategy Create(ApplicationReviewEntity applicationReview)
    {
        ArgumentNullException.ThrowIfNull(applicationReview, nameof(applicationReview));
        
        return applicationReview.Status switch {
            ApplicationReviewStatus.Shared => sharedApplicationEmailStrategy,
            ApplicationReviewStatus.EmployerInterviewing => employerHasReviewedApplicationEmailStrategy,
            ApplicationReviewStatus.EmployerUnsuccessful => employerHasReviewedApplicationEmailStrategy,
            _ => throw new MissingEmailStrategyException($"No registered handler for Application Review Status {applicationReview.Status}")
        };
    }
}