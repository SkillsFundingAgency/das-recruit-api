using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email.TemplateHandlers;

/// <summary>
/// This handler can deal with any RecruitNotification that only requires the Static data returned in the tokens
/// i.e. no dynamic data is required, which should only be an immediate email.
/// </summary>
public class StaticDataEmailHandler: AbstractEmailHandler
{
    public StaticDataEmailHandler(IEmailTemplateHelper emailTemplateHelper)
    {
        SupportedTemplates.Add(emailTemplateHelper.GetTemplateId(NotificationTypes.ApplicationSharedWithEmployer, NotificationFrequency.Immediately));
        SupportedTemplates.Add(emailTemplateHelper.GetTemplateId(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Immediately, UserType.Employer));
        SupportedTemplates.Add(emailTemplateHelper.GetTemplateId(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Immediately, UserType.Provider));
        SupportedTemplates.Add(emailTemplateHelper.GetTemplateId(NotificationTypes.SharedApplicationReviewedByEmployer, NotificationFrequency.Immediately));
        SupportedTemplates.Add(emailTemplateHelper.GetTemplateId(NotificationTypes.VacancySentForReview, NotificationFrequency.Immediately));
        SupportedTemplates.Add(emailTemplateHelper.GetTemplateId(NotificationTypes.VacancyApprovedOrRejected, NotificationFrequency.Immediately));
    }
    
    public override IEnumerable<NotificationEmail> CreateNotificationEmails(IEnumerable<RecruitNotificationEntity> recruitNotifications)
    {
        return recruitNotifications.Select(record => new NotificationEmail {
            TemplateId = record.EmailTemplateId,
            RecipientAddress = record.User.Email,
            Tokens = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(record.StaticData) ?? [],
            SourceIds = [record.Id]
        });
    }
}