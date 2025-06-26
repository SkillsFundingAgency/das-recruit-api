namespace SFA.DAS.Recruit.Api.Domain.Models;
public sealed record VacancyDashboardModel
{
    public int Total { get; init; } = 0;
    public int Page { get; set; } = 1;
    public List<long> VacancyReferences { get; set; } = [];
}
