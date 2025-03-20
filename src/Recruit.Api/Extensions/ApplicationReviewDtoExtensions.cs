using Recruit.Api.Domain.Entities;
using Recruit.Api.Domain.Enums;
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
            entity.Owner,
            entity.CandidateFeedback,
            entity.CandidateId,
            entity.CreatedDate,
            entity.DateSharedWithEmployer,
            entity.HasEverBeenEmployerInterviewing,
            entity.IsWithdrawn,
            entity.ReviewedDate,
            entity.SubmittedDate,
            (ApplicationStatus)entity.Status,
            entity.StatusUpdatedByUserId,
            entity.StatusUpdatedBy,
            entity.VacancyReference,
            entity.LegacyApplicationId);
    }

    public static ApplicationReviewEntity ToEntity(this ApplicationReviewRequest request)
    {
        return new ApplicationReviewEntity
        {
            Id = request.Id,
            AccountId = request.AccountId,
            CandidateFeedback = request.CandidateFeedback,
            CandidateId = request.CandidateId,
            CreatedDate = request.CreatedDate,
            DateSharedWithEmployer = request.DateSharedWithEmployer,
            HasEverBeenEmployerInterviewing = request.HasEverBeenEmployerInterviewing,
            IsWithdrawn = request.IsWithdrawn,
            Owner = request.Owner,
            ReviewedDate = request.ReviewedDate,
            Status = (short)request.Status,
            StatusUpdatedBy = request.StatusUpdatedBy,
            StatusUpdatedByUserId = request.StatusUpdatedByUserId,
            SubmittedDate = request.SubmittedDate,
            Ukprn = request.Ukprn,
            VacancyReference = request.VacancyReference,
            LegacyApplicationId = request.LegacyApplicationId

        };
    }
}