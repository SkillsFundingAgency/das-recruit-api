namespace SFA.DAS.Recruit.Api.Domain.Models;
public record VacancyDetail
{
    public long VacancyReference { get; set; }
    public int NewApplications { get; set; } = 0;
    public int Applications { get; set; } = 0;
    public int Shared { get; set; } = 0;
    public int AllSharedApplications { get; set; } = 0;
}