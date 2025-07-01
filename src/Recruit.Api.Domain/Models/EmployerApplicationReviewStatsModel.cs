using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Extensions;

namespace SFA.DAS.Recruit.Api.Domain.Models;
public record EmployerApplicationReviewStatsModel
{
    public int NewApplicationsCount { get; init; }
    public int EmployerReviewedApplicationsCount { get; init; }
    public int SuccessfulApplicationsCount { get; init; }
    public int UnsuccessfulApplicationsCount { get; init; }
    public int SharedApplicationsCount { get; set; }
    public bool HasNoApplications { get; init; }

    public static implicit operator EmployerApplicationReviewStatsModel(List<ApplicationReviewEntity> applicationReviews)
    {
        return new EmployerApplicationReviewStatsModel
        {
            NewApplicationsCount = applicationReviews.New(),
            EmployerReviewedApplicationsCount = applicationReviews.EmployerReviewed(),
            SharedApplicationsCount = applicationReviews.Shared(),
            SuccessfulApplicationsCount = applicationReviews.Successful(),
            UnsuccessfulApplicationsCount = applicationReviews.Unsuccessful(),
            HasNoApplications = applicationReviews.HasNoApplications()
        };
    }
}