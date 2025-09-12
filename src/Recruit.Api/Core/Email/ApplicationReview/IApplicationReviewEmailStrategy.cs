using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.ApplicationReview;

public interface IApplicationReviewEmailStrategy
{
    Task<List<NotificationEmail>> ExecuteAsync(ApplicationReviewEntity applicationReview, CancellationToken cancellationToken);
}