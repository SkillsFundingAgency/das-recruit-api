namespace Recruit.Api.Domain.Entities;

public class ApplicationReviewEntity
{
    public required Guid Id { get; set; }
    public required int Ukprn { get; set; }
    public required long AccountId { get; set; }
    public required short Owner { get; set; }
    public string? CandidateFeedback { get; set; }
    public Guid CandidateId { get; set; }
    public required DateTime CreatedDate { get; set; }
    public DateTime? DateSharedWithEmployer { get; set; }
    public required bool HasEverBeenEmployerInterviewing { get; set; }
    public required bool IsWithdrawn { get; set; }

    public DateTime? ReviewedDate { get; set; }
    public required DateTime SubmittedDate { get; set; }

    public required short Status { get; set; }
    public Guid StatusUpdatedByUserId { get; set; }
    public short? StatusUpdatedBy { get; set; }
    public required long VacancyReference { get; set; }
    public Guid LegacyApplicationId { get; set; }
}