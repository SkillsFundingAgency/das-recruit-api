using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.Vacancy;

public interface IVacancyNotificationFactory
{
    Task<RecruitNotificationsResult> CreateAsync(VacancyEntity vacancy, CancellationToken cancellationToken);
}