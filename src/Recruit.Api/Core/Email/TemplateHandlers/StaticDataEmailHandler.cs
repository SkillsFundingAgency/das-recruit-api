using SFA.DAS.Recruit.Api.Domain.Entities;
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
        SupportedTemplates.Add(emailTemplateHelper.TemplateIds.ApplicationSharedWithEmployer);
        SupportedTemplates.Add(emailTemplateHelper.TemplateIds.ApplicationSubmittedToProviderImmediate);
        SupportedTemplates.Add(emailTemplateHelper.TemplateIds.ApplicationSubmittedToEmployerImmediate);
        SupportedTemplates.Add(emailTemplateHelper.TemplateIds.SharedApplicationReviewedByEmployer);
        SupportedTemplates.Add(emailTemplateHelper.TemplateIds.ProviderVacancySentForEmployerReview);
        SupportedTemplates.Add(emailTemplateHelper.TemplateIds.ProviderVacancyApprovedByEmployer);
    }
    
    public override IEnumerable<NotificationEmail> CreateNotificationEmails(IEnumerable<RecruitNotificationEntity> recruitNotifications)
    {
        return recruitNotifications.Select(record => new NotificationEmail {
            TemplateId = record.EmailTemplateId,
            RecipientAddress = record.User.Email,
            Tokens = ApiUtils.DeserializeOrNull<Dictionary<string, string>>(record.StaticData) ?? [],
            SourceIds = record.Id > 0 ? [record.Id] : null
        });
    }
}