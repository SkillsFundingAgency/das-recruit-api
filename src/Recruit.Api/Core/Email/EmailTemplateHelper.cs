namespace SFA.DAS.Recruit.Api.Core.Email;

public class EmailTemplateHelper(string environmentName)
{
    private static readonly Dictionary<EmailTemplates, Guid> ProductionTemplates = new() {
        [EmailTemplates.ApplicationReviewShared] = new Guid("53058846-e369-4396-87b2-015c9d16360a"),
    };
    
    private static readonly Dictionary<EmailTemplates, Guid> DevelopmentTemplates = new() {
        [EmailTemplates.ApplicationReviewShared] = new Guid("f6fc57e6-7318-473d-8cb5-ca653035391a"),
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
}