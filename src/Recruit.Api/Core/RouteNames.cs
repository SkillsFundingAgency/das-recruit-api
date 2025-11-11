namespace SFA.DAS.Recruit.Api.Core;

internal struct RouteElements
{
    public const string Account = "accounts";
    public const string Api = "api";
    public const string ApplicationReview = "applicationreviews";
    public const string Employer = "employer";
    public const string EmployerProfileAddresses = "addresses";
    public const string EmployerProfiles = "profiles";
    public const string Notifications = "notifications";
    public const string ProhibitedContent = "prohibitedcontent";
    public const string Provider = "provider";
    public const string User = "user";
    public const string Vacancies = "vacancies";
    public const string VacancyReference = "vacancyreference";
    public const string VacancyReview = "vacancyreviews";
    public const string Reports = "reports";
    public const string Qa = "qa";
}

internal struct RouteNames
{
    public const string Account = $"{RouteElements.Api}/{RouteElements.Account}";
    public const string ApplicationReview = $"{RouteElements.Api}/{RouteElements.ApplicationReview}";
    public const string Employer = $"{RouteElements.Api}/{RouteElements.Employer}";
    public const string EmployerProfile = $"{RouteElements.Api}/{RouteElements.Employer}/{RouteElements.EmployerProfiles}";
    public const string Notifications = $"{RouteElements.Api}/{RouteElements.Notifications}";
    public const string ProhibitedContent = $"{RouteElements.Api}/{RouteElements.ProhibitedContent}";
    public const string Provider = $"{RouteElements.Api}/{RouteElements.Provider}";
    public const string User = $"{RouteElements.Api}/{RouteElements.User}";
    public const string VacancyReviews = $"{RouteElements.Api}/{RouteElements.VacancyReview}";
    public const string VacancyReference = $"{RouteElements.Api}/{RouteElements.VacancyReference}";
    public const string Vacancies = $"{RouteElements.Api}/{RouteElements.Vacancies}";
    public const string Reports = $"{RouteElements.Api}/{RouteElements.Reports}";
}