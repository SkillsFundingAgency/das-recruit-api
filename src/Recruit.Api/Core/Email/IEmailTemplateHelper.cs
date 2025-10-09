using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Core.Email;

public interface IEmailTemplateHelper
{
    string RecruitEmployerBaseUrl { get; }
    public string RecruitProviderBaseUrl { get; }
    Guid GetTemplateId(NotificationTypes notificationType, NotificationFrequency notificationFrequency);
    string ProviderManageNotificationsUrl(string ukprn);
    string EmployerManageNotificationsUrl(string hashedAccountId);
}