using SFA.DAS.Recruit.Api.Domain.Entities;

namespace SFA.DAS.Recruit.Api.Core.Email.NotificationGenerators.FeedbackNudgeEmail;

public interface IVacancyFeedbackNotificationFactory
{
    Task<RecruitNotificationsResult> CreateAsync(VacancyEntity vacancy, Dictionary<string, string> data, CancellationToken cancellationToken);
}