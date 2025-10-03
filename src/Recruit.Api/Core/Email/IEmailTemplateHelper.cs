using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Core.Email;

public interface IEmailTemplateHelper
{
    string RecruitEmployerBaseUrl { get; }
    public string RecruitProviderBaseUrl { get; }
    Guid GetTemplateId(NotificationTypes notificationType);
    Guid GetTemplateId(NotificationTypes notificationType, NotificationFrequency notificationFrequency);
    Guid GetTemplateId(NotificationTypes notificationType, NotificationFrequency notificationFrequency, UserType userType);
    string ProviderManageNotificationsUrl(string ukprn);
    string ProviderManageVacancyUrl(string ukprn, Guid vacancyId);
    string EmployerManageNotificationsUrl(string hashedAccountId);
    string EmployerManageVacancyUrl(string hashedAccountId, Guid vacancyId);
}