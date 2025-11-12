namespace SFA.DAS.Recruit.Api.Core.Email;

public interface IRecruitBaseUrls
{
    string RecruitEmployerBaseUrl { get; }
    string RecruitProviderBaseUrl { get; }
}

public class ProductionRecruitBaseUrls: IRecruitBaseUrls
{
    public string RecruitEmployerBaseUrl { get; } = "https://recruit.manage-apprenticeships.service.gov.uk";
    public string RecruitProviderBaseUrl { get; } = "https://recruit.providers.apprenticeships.education.gov.uk";
}

public class DevelopmentRecruitBaseUrls(string environmentName) : IRecruitBaseUrls
{
    public string RecruitEmployerBaseUrl { get; } = $"https://recruit.{environmentName.ToLower()}-eas.apprenticeships.education.gov.uk";
    public string RecruitProviderBaseUrl { get; } = $"https://recruit.{environmentName.ToLower()}-pas.apprenticeships.education.gov.uk";
}
