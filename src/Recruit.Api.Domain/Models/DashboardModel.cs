﻿namespace SFA.DAS.Recruit.Api.Domain.Models
{
    public record DashboardModel
    {
        public int NewApplicationsCount { get; init; } = 0;
        public int EmployerReviewedApplicationsCount { get; init; } = 0;
        public int SuccessfulApplicationsCount { get; init; } = 0;
        public int UnsuccessfulApplicationsCount { get; init; } = 0;
        public int SharedApplicationsCount { get; set; } = 0;
        public int AllSharedApplicationsCount { get; set; } = 0;
        public bool HasNoApplications { get; init; } = false;
    }
}