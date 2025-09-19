using SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email;

public interface IEmailFactory
{
    List<NotificationEmail> CreateFrom(IEnumerable<RecruitNotificationEntity> notifications);
}

public class EmailFactory(IEnumerable<IEmailTemplateHandler> templateHandlers): IEmailFactory
{
    public List<NotificationEmail> CreateFrom(IEnumerable<RecruitNotificationEntity> notifications)
    {
        List<NotificationEmail> results = [];
        var groupsByTemplateId = notifications.GroupBy(x => x.EmailTemplateId);
        foreach (var templateGroup in groupsByTemplateId)
        {
            var records = templateHandlers
                .Where(x => x.CanHandle(templateGroup.Key))
                .SelectMany(x => x.CreateNotificationEmails(templateGroup));
            results.AddRange(records);
        }
        return results;
    }
} 