using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Extensions;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;

/// <summary>
/// This handler can deal with any RecruitNotification that only requires the Static data returned in the tokens
/// i.e. no dynamic data is required, which is normally an immediate email
/// </summary>
public class StaticDataEmailHandler: AbstractEmailHandler
{
    public StaticDataEmailHandler(IEmailTemplateHelper emailTemplateHelper)
    {
        SupportedTemplates.Add(emailTemplateHelper.GetTemplateId(NotificationTypes.ApplicationSharedWithEmployer, NotificationFrequency.Immediately));
        SupportedTemplates.Add(emailTemplateHelper.GetTemplateId(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Immediately));
        SupportedTemplates.Add(emailTemplateHelper.GetTemplateId(NotificationTypes.SharedApplicationReviewedByEmployer, NotificationFrequency.Immediately));
    }
    
    public override IEnumerable<NotificationEmail> CreateNotificationEmails(IEnumerable<RecruitNotificationEntity> recruitNotifications)
    {
        var groupedByUser = recruitNotifications.GroupBy(x => x.UserId);
        List<NotificationEmail> results = [];
        foreach (var userGroup in groupedByUser)
        {
            foreach (var record in userGroup)
            {
                var staticData = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(record.StaticData) ?? [];
                var notificationEmail = new NotificationEmail {
                    TemplateId = record.EmailTemplateId,
                    RecipientAddress = record.User.Email,
                    Tokens = staticData
                };

                results.Add(notificationEmail);
            }
        }

        return results;
    }
}