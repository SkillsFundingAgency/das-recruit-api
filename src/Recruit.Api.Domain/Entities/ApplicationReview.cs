using Recruit.Api.Domain.Enums;

namespace Recruit.Api.Domain.Entities;

public record ApplicationReview : ApplicationBase
{
    public Guid Id { get; set; }
    public int Ukprn { get; set; }
    public long AccountId { get; set; }
    public Guid CandidateId { get; init; }
    public short Owner { get; init; }
    public string? CandidateFeedback { get; init; }
    public DateTime? DateSharedWithEmployer { get; init; }
    public bool HasEverBeenEmployerInterviewing { get; init; }
    public bool IsWithdrawn { get; init; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ReviewedDate { get; init; }
    public DateTime SubmittedDate { get; init; }
    public Guid StatusUpdatedByUserId { get; init; }
    public short? StatusUpdatedBy { get; init; }
    public long VacancyReference { get; init; }
    public Guid LegacyApplicationId { get; init; }

    public static implicit operator ApplicationReview(ApplicationReviewEntity entity)
    {
        return new ApplicationReview 
        {
            Id = entity.Id,
            Ukprn = entity.Ukprn,
            AccountId = entity.AccountId,
            Owner = entity.Owner,
            CandidateFeedback = entity.CandidateFeedback,
            CandidateId = entity.CandidateId,
            CreatedDate = entity.CreatedDate,
            DateSharedWithEmployer = entity.DateSharedWithEmployer,
            HasEverBeenEmployerInterviewing = entity.HasEverBeenEmployerInterviewing,
            IsWithdrawn = entity.IsWithdrawn,
            ReviewedDate = entity.ReviewedDate,
            SubmittedDate = entity.SubmittedDate,
            Status = ParseValue<ApplicationStatus>(entity.Status),
            StatusUpdatedByUserId = entity.StatusUpdatedByUserId,
            StatusUpdatedBy = entity.StatusUpdatedBy,
            VacancyReference = entity.VacancyReference,
            LegacyApplicationId = entity.LegacyApplicationId
        };
    }
}