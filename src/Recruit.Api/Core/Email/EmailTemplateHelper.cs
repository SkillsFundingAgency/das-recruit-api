using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Core.Email;

public class EmailTemplateHelper: IEmailTemplateHelper
{
    private record TemplateKey(NotificationTypes Type, NotificationFrequency Frequency);
    private readonly Dictionary<TemplateKey, Guid> _templateMap;
    
    public string RecruitEmployerBaseUrl { get; }
    public string RecruitProviderBaseUrl { get; }
    
    public EmailTemplateHelper(string environmentName)
    {
        if (environmentName.Equals("PRD", StringComparison.CurrentCultureIgnoreCase))
        {
            // PRODUCTION templates & properties
            _templateMap = new Dictionary<TemplateKey, Guid> {
                [new TemplateKey(NotificationTypes.ApplicationSharedWithEmployer, NotificationFrequency.Immediately)] = new("53058846-e369-4396-87b2-015c9d16360a"),
                [new TemplateKey(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Immediately)] = new("e07a6992-4d17-4167-b526-2ead6fe9ad4d"),
                [new TemplateKey(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Daily)] = new("1c8c9e72-86c1-4fd1-8020-f4fe354a6e79"),
                [new TemplateKey(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Weekly)] = new("68d467ac-339c-42b4-b862-ca06e1cc66e8"),
                [new TemplateKey(NotificationTypes.SharedApplicationReviewedByEmployer, NotificationFrequency.Immediately)] = new("2f1b70d4-c722-4815-85a0-80a080eac642"),
                [new TemplateKey(NotificationTypes.VacancySentForReview, NotificationFrequency.Immediately)] = new("2b69c0b2-bcc0-4988-82b6-868874e5617b"),
            };

            RecruitEmployerBaseUrl = "https://recruit.manage-apprenticeships.service.gov.uk";
            RecruitProviderBaseUrl = "https://recruit.providers.apprenticeships.education.gov.uk";
        }
        else
        {
            // Dev templates & properties
            _templateMap = new Dictionary<TemplateKey, Guid> {
                [new TemplateKey(NotificationTypes.ApplicationSharedWithEmployer, NotificationFrequency.Immediately)] = new("f6fc57e6-7318-473d-8cb5-ca653035391a"),
                [new TemplateKey(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Immediately)] = new("8aedd294-fd12-4b77-b4b8-2066744e1fdc"),
                [new TemplateKey(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Daily)] = new("b793a50f-49f0-4b3f-a4c3-46a8f857e48c"),
                [new TemplateKey(NotificationTypes.ApplicationSubmitted, NotificationFrequency.Weekly)] = new("520a434a-2203-49f6-a15a-9e9d1c58c18f"),
                [new TemplateKey(NotificationTypes.SharedApplicationReviewedByEmployer, NotificationFrequency.Immediately)] = new("feb4191d-a373-4040-9bc6-93c09d8039b5"),
                [new TemplateKey(NotificationTypes.VacancySentForReview, NotificationFrequency.Immediately)] = new("83f6cede-31c3-4dc9-b2ec-922856ba9bdc"),
            };
            
            RecruitEmployerBaseUrl = $"https://recruit.{environmentName.ToLower()}-eas.apprenticeships.education.gov.uk";
            RecruitProviderBaseUrl = $"https://recruit.{environmentName.ToLower()}-pas.apprenticeships.education.gov.uk";
        }
    }
    
    public Guid GetTemplateId(NotificationTypes notificationType, NotificationFrequency notificationFrequency)
    {
        return _templateMap.TryGetValue(new TemplateKey(notificationType, notificationFrequency), out var templateId)
            ? templateId
            : throw new ArgumentException($"Template has not been defined for the notification type '{notificationType}' with frequency '{notificationFrequency}'");
    }
    
    public string ProviderManageNotificationsUrl(string ukprn) => $"{RecruitProviderBaseUrl}/{ukprn}/notifications-manage";
    public string ProviderManageVacancyUrl(string ukprn, Guid vacancyId) => $"{RecruitProviderBaseUrl}/{ukprn}/vacancies/{vacancyId}/manage";

    public string EmployerManageNotificationsUrl(string hashedAccountId) => $"{RecruitEmployerBaseUrl}/accounts/{hashedAccountId}/notifications-manage";
    public string EmployerManageVacancyUrl(string hashedAccountId, Guid vacancyId) => $"{RecruitEmployerBaseUrl}/accounts/{hashedAccountId}/vacancies/{vacancyId}/manage";
}