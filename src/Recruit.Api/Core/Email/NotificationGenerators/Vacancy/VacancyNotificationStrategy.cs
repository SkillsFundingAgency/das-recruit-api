using SFA.DAS.Recruit.Api.Core.Exceptions;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;

public interface IVacancyNotificationStrategy
{
    IVacancyNotificationFactory Create(VacancyEntity vacancy);
}

public class VacancyNotificationStrategy(
    VacancyRejectedNotificationFactory vacancyRejectedNotificationFactory,
    VacancySentForReviewNotificationFactory vacancySentForReviewNotificationFactory,
    VacancySubmittedNotificationFactory vacancySubmittedNotificationFactory,
    VacancyApprovedNotificationFactory vacancyApprovedNotificationFactory,
    VacancyReferredNotificationFactory vacancyReferredNotificationFactory) : IVacancyNotificationStrategy
{
    public IVacancyNotificationFactory Create(VacancyEntity vacancy)
    {
        ArgumentNullException.ThrowIfNull(vacancy);

        return vacancy.Status switch {
            VacancyStatus.Rejected => vacancyRejectedNotificationFactory,
            VacancyStatus.Review => vacancySentForReviewNotificationFactory,
            VacancyStatus.Submitted => vacancySubmittedNotificationFactory,
            VacancyStatus.Approved => vacancyApprovedNotificationFactory,
            VacancyStatus.Referred => vacancyReferredNotificationFactory,
            _ => throw new EntityStateNotSupportedException($"Missing email handler: no registered handler for Vacancy Status {vacancy.Status}")
        };
    }
}