namespace SFA.DAS.Recruit.Api.Core.Email;

public class EmailTemplateHelper(string environmentName)
{
    // ====================================================
    // PRODUCTION templates 
    // ====================================================
    private static readonly Dictionary<EmailTemplates, Guid> ProductionTemplates = new() {
        [EmailTemplates.ApplicationReviewShared] = new Guid("53058846-e369-4396-87b2-015c9d16360a"),
        [EmailTemplates.EmployerHasReviewedSharedApplication] = new Guid("2f1b70d4-c722-4815-85a0-80a080eac642"),
    };
    
    // ====================================================
    // Development templates 
    // ====================================================
    private static readonly Dictionary<EmailTemplates, Guid> DevelopmentTemplates = new() {
        [EmailTemplates.ApplicationReviewShared] = new Guid("f6fc57e6-7318-473d-8cb5-ca653035391a"),
        [EmailTemplates.EmployerHasReviewedSharedApplication] = new Guid("feb4191d-a373-4040-9bc6-93c09d8039b5"),
    };

    private readonly bool _isProduction = environmentName.Equals("PRD", StringComparison.CurrentCultureIgnoreCase);
    
    public Guid GetTemplateId(EmailTemplates template)
    {
        var templates = _isProduction ? ProductionTemplates : DevelopmentTemplates;
        return templates.TryGetValue(template, out var templateId)
            ? templateId
            : throw new ArgumentException($"Template has not been defined for the specified value '{template}'");
    }

    public string RecruitEmployerBaseUrl => _isProduction 
        ? "https://recruit.manage-apprenticeships.service.gov.uk"
        : $"https://recruit.{environmentName.ToLower()}-eas.apprenticeships.education.gov.uk";
    
    public string RecruitProviderBaseUrl => _isProduction 
        ? "https://recruit.providers.apprenticeships.education.gov.uk"
        : $"https://recruit.{environmentName.ToLower()}-pas.apprenticeships.education.gov.uk";

    public string ProviderManageNotificationsUrl(string ukprn) => $"{RecruitProviderBaseUrl}/{ukprn}/notifications-manage";
    public string EmployerManageNotificationsUrl(string hashedAccountId) => $"{RecruitEmployerBaseUrl}/accounts/{hashedAccountId}/notifications-manage";
}