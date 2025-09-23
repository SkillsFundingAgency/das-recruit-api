using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.ApplicationReview;

public interface IApplicationReviewNotificationFactory
{
    Task<RecruitNotificationsResult> CreateAsync(ApplicationReviewEntity applicationReview, CancellationToken cancellationToken);
}