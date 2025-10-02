using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;

public interface IVacancyNotificationStrategy
{
    IVacancyNotificationFactory Create(VacancyEntity vacancy);
}

public class VacancyNotificationStrategy(
    VacancySentForReviewNotificationFactory vacancySentForReviewNotificationFactory) : IVacancyNotificationStrategy
{
    public IVacancyNotificationFactory Create(VacancyEntity vacancy)
    {
        ArgumentNullException.ThrowIfNull(vacancy, nameof(vacancy));

        return vacancy.Status switch {
            VacancyStatus.Review => vacancySentForReviewNotificationFactory,
            _ => throw new NotSupportedException($"Missing email handler: no registered handler for Vacancy Status {vacancy.Status}")
        };
    }
}