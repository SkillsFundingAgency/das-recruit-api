using System.ComponentModel.DataAnnotations;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Domain.Entities;

public class VacancyReviewEntity
{
    [Key]
    public Guid Id { get; init; }
    public required VacancyReference VacancyReference { get; set; }
    [MaxLength(500)]
    public required string VacancyTitle { get; init; }
    public required DateTime CreatedDate { get; init; }
    public required DateTime SlaDeadLine { get; init; }
    public DateTime? ReviewedDate { get; init; }
    public required ReviewStatus Status { get; init; }
    public byte SubmissionCount { get; init; }
    [MaxLength(255)]
    public string? ReviewedByUserEmail { get; init; }
    [MaxLength(255)]
    public required string SubmittedByUserEmail { get; init; }
    public DateTime? ClosedDate { get; init; }
    [MaxLength(50)]
    public string? ManualOutcome { get; set; }
    public string? ManualQaComment { get; init; }
    public required string ManualQaFieldIndicators { get; init; }
    public string? AutomatedQaOutcome { get; init; }
    [MaxLength(20)]
    public string? AutomatedQaOutcomeIndicators { get; init; }
    public required string DismissedAutomatedQaOutcomeIndicators { get; init; }
    public required string UpdatedFieldIdentifiers { get; init; }
    public required string VacancySnapshot { get; init; }
    public required long AccountId { get; init; }
    public required long AccountLegalEntityId { get; init; }
    public required long Ukprn { get; init; }
    public required OwnerType OwnerType { get; init; }
}