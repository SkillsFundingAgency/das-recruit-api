namespace Recruit.Api.Domain.Entities;

public record ApplicationReview : ApplicationBase
{
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