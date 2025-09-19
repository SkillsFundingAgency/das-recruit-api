using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;

public interface IEmailTemplateHandler
{
    bool CanHandle(Guid templateId);
    IEnumerable<NotificationEmail> CreateNotificationEmails(IEnumerable<RecruitNotificationEntity> recruitNotifications);
}