namespace SFA.DAS.Recruit.Api.Core;

internal struct RouteElements
{
    public const string Api = "api";
    public const string ApplicationReview = "applicationreviews";
    public const string Employer = "employer";
    public const string EmployerProfileAddresses = "addresses";
    public const string EmployerProfiles = "profiles";
    public const string ProhibitedContent = "prohibitedcontent";
    public const string Provider = "provider";
}

internal struct RouteNames
{
    public const string ApplicationReview = $"{RouteElements.Api}/{RouteElements.ApplicationReview}";
    public const string Employer = $"{RouteElements.Api}/{RouteElements.Employer}";
    public const string EmployerProfile = $"{RouteElements.Api}/{RouteElements.Employer}/{RouteElements.EmployerProfiles}";
    public const string ProhibitedContent = $"{RouteElements.Api}/{RouteElements.ProhibitedContent}";
    public const string Provider = $"{RouteElements.Api}/{RouteElements.Provider}";
}