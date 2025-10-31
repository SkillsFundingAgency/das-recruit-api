namespace SFA.DAS.Recruit.Api.Core.Email;

public interface IEmailTemplateHelper: IRecruitBaseUrls
{
    IEmailTemplateIds TemplateIds { get; }
    string ProviderManageNotificationsUrl(string ukprn);
    string ProviderManageVacancyUrl(string ukprn, Guid vacancyId);
    string EmployerManageNotificationsUrl(string hashedAccountId);
    string EmployerManageVacancyUrl(string hashedAccountId, Guid vacancyId);
}