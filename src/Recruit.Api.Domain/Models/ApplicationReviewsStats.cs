﻿namespace Recruit.Api.Domain.Models
{
    public record ApplicationReviewsStats
    {
        public long VacancyReference { get; init; }
        public int NewApplications { get; init; }
        public int SuccessfulApplications { get; init; }
        public int UnsuccessfulApplications { get; init; }
        public int Applications { get; init; }
    }
}
