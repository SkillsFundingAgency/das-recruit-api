namespace SFA.DAS.Recruit.Api.Domain.Models;

public record QaDashboard
{
    public int TotalVacanciesForReview { get; set; } = 0;
    public int TotalVacanciesBrokenSla { get; set; } = 0;
    public int TotalVacanciesResubmitted { get; set; } = 0;
    public int TotalVacanciesSubmittedTwelveTwentyFourHours { get; set; } = 0;
}