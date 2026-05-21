using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Core.Email;

public interface IEmailTemplateHelper: IRecruitBaseUrls, IFaaBaseUrl
{
    IEmailTemplateIds TemplateIds { get; }
    string ProviderManageNotificationsUrl(string ukprn);
    string ProviderManageVacancyUrl(string ukprn, Guid vacancyId);
    string EmployerManageNotificationsUrl(string hashedAccountId);
    string EmployerManageVacancyUrl(string hashedAccountId, Guid vacancyId);
    string FaaVacancyUrl(VacancyReference vacancyReference);
    string ProviderReviewVacancyUrl(int ukprn, Guid vacancyId);
    string EmployerReviewVacancyUrl(string hashedAccountId, Guid vacancyId);
}