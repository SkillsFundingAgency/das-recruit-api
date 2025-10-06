namespace SFA.DAS.Recruit.Api.Core.Email;

public interface IEmailTemplateHelper
{
    IEmailTemplateIds TemplateIds { get; }
    string RecruitEmployerBaseUrl { get; }
    string RecruitProviderBaseUrl { get; }
    string ProviderManageNotificationsUrl(string ukprn);
    string ProviderManageVacancyUrl(string ukprn, Guid vacancyId);
    string EmployerManageNotificationsUrl(string hashedAccountId);
    string EmployerManageVacancyUrl(string hashedAccountId, Guid vacancyId);
}