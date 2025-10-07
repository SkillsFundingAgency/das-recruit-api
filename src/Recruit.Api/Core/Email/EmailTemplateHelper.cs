namespace SFA.DAS.Recruit.Api.Core.Email;

public class EmailTemplateHelper(IEmailTemplateIds emailTemplateIds, IRecruitBaseUrls recruitBaseUrls): IEmailTemplateHelper
{
    public string RecruitEmployerBaseUrl => recruitBaseUrls.RecruitEmployerBaseUrl;
    public string RecruitProviderBaseUrl => recruitBaseUrls.RecruitProviderBaseUrl;
    public IEmailTemplateIds TemplateIds { get; } = emailTemplateIds;
    public string ProviderManageNotificationsUrl(string ukprn) => $"{RecruitProviderBaseUrl}/{ukprn}/notifications-manage";
    public string ProviderManageVacancyUrl(string ukprn, Guid vacancyId) => $"{RecruitProviderBaseUrl}/{ukprn}/vacancies/{vacancyId}/manage";
    public string EmployerManageNotificationsUrl(string hashedAccountId) => $"{RecruitEmployerBaseUrl}/accounts/{hashedAccountId}/notifications-manage";
    public string EmployerManageVacancyUrl(string hashedAccountId, Guid vacancyId) => $"{RecruitEmployerBaseUrl}/accounts/{hashedAccountId}/vacancies/{vacancyId}/manage";
}