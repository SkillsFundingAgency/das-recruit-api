using Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Requests;
using SFA.DAS.Recruit.Api.Models.Responses;

namespace SFA.DAS.Recruit.Api.Extensions;

public static class ApplicationReviewDtoExtensions
{
    public static ApplicationReviewResponse ToResponse(this ApplicationReviewEntity entity)
    {
        return new ApplicationReviewResponse(entity.Id,
            entity.Ukprn,
            entity.AccountId,
            entity.AccountLegalEntityId,
            entity.Owner,
            entity.CandidateFeedback,
            entity.EmployerFeedback,
            entity.CandidateId,
            entity.CreatedDate,
            entity.DateSharedWithEmployer,
            entity.HasEverBeenEmployerInterviewing,
            entity.WithdrawnDate,
            entity.ReviewedDate,
            entity.SubmittedDate,
            entity.Status,
            entity.StatusUpdatedDate,
            entity.VacancyReference,
            entity.LegacyApplicationId,
            entity.ApplicationId,
            entity.AdditionalQuestion1,
            entity.AdditionalQuestion2,
            entity.VacancyTitle);
    }

    public static ApplicationReviewEntity ToEntity(this ApplicationReviewRequest request)
    {
        return new ApplicationReviewEntity
        {
            Id = request.Id,
            Ukprn = request.Ukprn,
            AccountId = request.AccountId,
            Owner = request.Owner,
            CandidateFeedback = request.CandidateFeedback,
            CandidateId = request.CandidateId,
            CreatedDate = request.CreatedDate,
            DateSharedWithEmployer = request.DateSharedWithEmployer,
            HasEverBeenEmployerInterviewing = request.HasEverBeenEmployerInterviewing,
            ReviewedDate = request.ReviewedDate,
            Status = request.Status,
            StatusUpdatedDate = request.StatusUpdatedDate,
            SubmittedDate = request.SubmittedDate,
            VacancyReference = request.VacancyReference,
            LegacyApplicationId = request.LegacyApplicationId,
            ApplicationId = request.ApplicationId,
            AdditionalQuestion1 = request.AdditionalQuestion1,
            AdditionalQuestion2 = request.AdditionalQuestion2,
            VacancyTitle = request.VacancyTitle,
            AccountLegalEntityId = request.AccountLegalEntityId,
            EmployerFeedback = request.EmployerFeedback,
            WithdrawnDate = request.WithdrawnDate
        };
    }
}