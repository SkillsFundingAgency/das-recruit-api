namespace Recruit.Api.Domain.Models
{
    public record ApplicationReviewsStats
    {
        public long VacancyReference { get; set; }
        public int NewApplications { get; set; }
        public int TotalApplication { get; set; }
    }
}
