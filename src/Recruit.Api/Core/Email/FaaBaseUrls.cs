namespace SFA.DAS.Recruit.Api.Core.Email;

public interface IFaaBaseUrl
{
    string FaaBaseUrl { get; }
}

public class ProductionFaaBaseUrls: IFaaBaseUrl
{
    public string FaaBaseUrl => "https://findapprenticeship.service.gov.uk";
}

public class DevelopmentFaaBaseUrls(string environmentName): IFaaBaseUrl
{
    public string FaaBaseUrl => $"https://{environmentName.ToLower()}-findapprenticeship.apprenticeships.education.gov.uk";
}