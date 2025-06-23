using System.ComponentModel.DataAnnotations;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Models.Requests.VacancyReview;

public class PutVacancyReviewRequest
{
    [Required]
    public required VacancyReference? VacancyReference { get; init; }
    public required string VacancyTitle { get; init; }
    public required DateTime? CreatedDate { get; init; }
    public required DateTime? SlaDeadLine { get; init; }
    public DateTime? ReviewedDate { get; init; }
    public required ReviewStatus Status { get; init; }
    public required byte? SubmissionCount { get; init; }
    public string? ReviewedByUserEmail { get; init; }
    public required string SubmittedByUserEmail { get; init; }
    public DateTime? ClosedDate { get; init; }
    public string? ManualOutcome { get; init; }
    public string? ManualQaComment { get; init; }
    public List<string>? ManualQaFieldIndicators { get; init; } = [];
    public string? AutomatedQaOutcome { get; init; }
    public string? AutomatedQaOutcomeIndicators { get; init; }
    public List<string>? DismissedAutomatedQaOutcomeIndicators { get; init; } = [];
    public List<string>? UpdatedFieldIdentifiers { get; init; } = [];
    public required string VacancySnapshot { get; init; }
    public long Ukprn { get; set; }
    public long AccountId { get; set; }
    public long AccountLegalEntityId { get; set; }
    public OwnerType OwnerType { get; set; }
    public DateTime? VacancyClosingDate { get; set; }
}