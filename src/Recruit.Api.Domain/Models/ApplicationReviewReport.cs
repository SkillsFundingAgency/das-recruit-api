using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Domain.Models;
public record ApplicationReviewReport
{
    public Guid ApplicationId { get; set; }
    public Guid CandidateId { get; set; }
    public long VacancyReference { get; set; }
    public string? VacancyTitle { get; set; }
    public string? EmployerName { get; set; }
    public string? TrainingProviderName { get; set; }
    public string? ProgrammeId { get; set; }
    public DateTime? VacancyClosingDate { get; set; }
    public DateTime? ApplicationSubmittedDate { get; set; }
    public AvailableWhere? AvailableWhere { get; set; }
    public ApplicationReviewStatus ApplicationStatus { get; set; }
    public int? NumberOfDaysApplicationAtThisStatus { get; set; }
}