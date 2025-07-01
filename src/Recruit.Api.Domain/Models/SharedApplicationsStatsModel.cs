using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Extensions;

namespace SFA.DAS.Recruit.Api.Domain.Models
{
    public record SharedApplicationReviewsStatsModel
    {
        public int AllSharedApplicationsCount { get; set; }

        public static implicit operator SharedApplicationReviewsStatsModel(List<ApplicationReviewEntity> applicationReviews)
        {
            return new SharedApplicationReviewsStatsModel
            {
                AllSharedApplicationsCount = applicationReviews.AllShared()
            };
        }
    }
}