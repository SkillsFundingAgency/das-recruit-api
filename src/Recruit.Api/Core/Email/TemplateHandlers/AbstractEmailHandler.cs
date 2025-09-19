using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;

public abstract class AbstractEmailHandler : IEmailTemplateHandler
{
    protected List<Guid> SupportedTemplates { get; } = [];
        
    public bool CanHandle(Guid templateId)
    {
        return SupportedTemplates.Contains(templateId);
    }

    public abstract IEnumerable<NotificationEmail> CreateNotificationEmails(IEnumerable<RecruitNotificationEntity> recruitNotifications);
}