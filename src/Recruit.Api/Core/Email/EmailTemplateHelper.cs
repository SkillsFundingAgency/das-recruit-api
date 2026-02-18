using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email;

public class EmailTemplateHelper(IEmailTemplateIds emailTemplateIds, IRecruitBaseUrls recruitBaseUrls, IFaaBaseUrl faaBaseUrl): IEmailTemplateHelper
{
    public string FaaBaseUrl => faaBaseUrl.FaaBaseUrl;
    public string RecruitEmployerBaseUrl => recruitBaseUrls.RecruitEmployerBaseUrl;
    public string RecruitProviderBaseUrl => recruitBaseUrls.RecruitProviderBaseUrl;
    public IEmailTemplateIds TemplateIds { get; } = emailTemplateIds;
    public string ProviderManageNotificationsUrl(string ukprn) => $"{RecruitProviderBaseUrl}/{ukprn}/notifications-manage";
    public string ProviderManageVacancyUrl(string ukprn, Guid vacancyId) => $"{RecruitProviderBaseUrl}/{ukprn}/vacancies/{vacancyId}/manage";
    public string EmployerManageNotificationsUrl(string hashedAccountId) => $"{RecruitEmployerBaseUrl}/accounts/{hashedAccountId}/notifications-manage";
    public string EmployerManageVacancyUrl(string hashedAccountId, Guid vacancyId) => $"{RecruitEmployerBaseUrl}/accounts/{hashedAccountId}/vacancies/{vacancyId}/manage";
    public string FaaVacancyUrl(VacancyReference vacancyReference) => $"{FaaBaseUrl}/apprenticeship/{vacancyReference}";
    public string ProviderReviewVacancyUrl(int ukprn, Guid vacancyId) => $"{RecruitProviderBaseUrl}/{ukprn}/vacancies/{vacancyId}/check-your-answers";
    public string EmployerReviewVacancyUrl(string hashedAccountId, Guid vacancyId) => $"{RecruitEmployerBaseUrl}/accounts/{hashedAccountId}/vacancies/{vacancyId}/check-answers";
}