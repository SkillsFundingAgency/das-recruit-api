using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Extensions;

namespace SFA.DAS.Recruit.Api.Domain.Models;
public record ProviderApplicationReviewStatsModel : SharedApplicationReviewsStatsModel
{
    public int NewApplicationsCount { get; init; }
    public int EmployerReviewedApplicationsCount { get; init; }
    public int SuccessfulApplicationsCount { get; init; }
    public int UnsuccessfulApplicationsCount { get; init; }
    public int SharedApplicationsCount { get; set; }
    public bool HasNoApplications { get; init; }

    public static implicit operator ProviderApplicationReviewStatsModel(List<ApplicationReviewEntity> applicationReviews)
    {
        return new ProviderApplicationReviewStatsModel
        {
            NewApplicationsCount = applicationReviews.New(),
            EmployerReviewedApplicationsCount = applicationReviews.EmployerReviewed(),
            SharedApplicationsCount = applicationReviews.Shared(),
            AllSharedApplicationsCount = applicationReviews.AllShared(),
            SuccessfulApplicationsCount = applicationReviews.Successful(),
            UnsuccessfulApplicationsCount = applicationReviews.Unsuccessful(),
            HasNoApplications = applicationReviews.HasNoApplications()
        };
    }
}