namespace Recruit.Api.Domain.Entities;

public record ApplicationReview
{
    public bool HasEverBeenEmployerInterviewing { get; init; }
    public DateTime CreatedDate { get; set; }
    public DateTime SubmittedDate { get; init; }
    public DateTime WithdrawnDate { get; set; }
    public DateTime? DateSharedWithEmployer { get; init; }
    public DateTime? ReviewedDate { get; init; }
    public DateTime? StatusUpdatedDate { get; init; }
    public Guid CandidateId { get; init; }
    public Guid Id { get; set; }
    public Guid? ApplicationId { get; init; } 
    public Guid? LegacyApplicationId { get; init; }
    public int Ukprn { get; set; }
    public long AccountId { get; set; }
    public long AccountLegalEntityId { get; set; }
    public long VacancyReference { get; init; }
    public required string VacancyTitle { get; init; }
    public short Owner { get; init; }
    public string? AdditionalQuestion1 { get; init; }
    public string? AdditionalQuestion2 { get; init; }
    public string? CandidateFeedback { get; init; }
    public string? EmployerFeedback { get; init; }
    public required string Status { get; init; }

    public static implicit operator ApplicationReview(ApplicationReviewEntity entity)
    {
        return new ApplicationReview {
            Id = entity.Id,
            Ukprn = entity.Ukprn,
            AccountId = entity.AccountId,
            Owner = entity.Owner,
            CandidateFeedback = entity.CandidateFeedback,
            CandidateId = entity.CandidateId,
            CreatedDate = entity.CreatedDate,
            DateSharedWithEmployer = entity.DateSharedWithEmployer,
            HasEverBeenEmployerInterviewing = entity.HasEverBeenEmployerInterviewing,
            ReviewedDate = entity.ReviewedDate,
            SubmittedDate = entity.SubmittedDate,
            VacancyReference = entity.VacancyReference,
            LegacyApplicationId = entity.LegacyApplicationId,
            VacancyTitle = entity.VacancyTitle,
            AccountLegalEntityId = entity.AccountLegalEntityId,
            AdditionalQuestion1 = entity.AdditionalQuestion1,
            AdditionalQuestion2 = entity.AdditionalQuestion2,
            EmployerFeedback = entity.EmployerFeedback,
            StatusUpdatedDate = entity.StatusUpdatedDate,
            ApplicationId = entity.ApplicationId,
            WithdrawnDate = entity.WithdrawnDate,
            Status = entity.Status
        };
    }
}