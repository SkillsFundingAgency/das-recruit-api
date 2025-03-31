using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests;
using SFA.DAS.Recruit.Api.Models.Responses;

namespace SFA.DAS.Recruit.Api.Extensions;

public static class ApplicationReviewExtensions
{
    public static ApplicationReview ToApplicationReview(this ApplicationReviewEntity entity)
    {
        return new ApplicationReview {
            AccountId = entity.AccountId,
            AccountLegalEntityId = entity.AccountLegalEntityId,
            AdditionalQuestion1 = entity.AdditionalQuestion1,
            AdditionalQuestion2 = entity.AdditionalQuestion2,
            ApplicationId = entity.ApplicationId,
            CandidateFeedback = entity.CandidateFeedback,
            CandidateId = entity.CandidateId,
            CreatedDate = entity.CreatedDate,
            DateSharedWithEmployer = entity.DateSharedWithEmployer,
            EmployerFeedback = entity.EmployerFeedback,
            HasEverBeenEmployerInterviewing = entity.HasEverBeenEmployerInterviewing,
            Id = entity.Id,
            LegacyApplicationId = entity.LegacyApplicationId,
            Owner = entity.Owner,
            ReviewedDate = entity.ReviewedDate,
            Status = entity.Status,
            StatusUpdatedDate = entity.StatusUpdatedDate,
            SubmittedDate = entity.SubmittedDate,
            Ukprn = entity.Ukprn,
            VacancyReference = entity.VacancyReference,
            VacancyTitle = entity.VacancyTitle,
            WithdrawnDate = entity.WithdrawnDate,
        };
    }
    
    public static GetApplicationReviewResponse ToGetResponse(this ApplicationReviewEntity entity)
    {
        return new GetApplicationReviewResponse {
            AccountId = entity.AccountId,
            AccountLegalEntityId = entity.AccountLegalEntityId,
            AdditionalQuestion1 = entity.AdditionalQuestion1,
            AdditionalQuestion2 = entity.AdditionalQuestion2,
            ApplicationId = entity.ApplicationId,
            CandidateFeedback = entity.CandidateFeedback,
            CandidateId = entity.CandidateId,
            CreatedDate = entity.CreatedDate,
            DateSharedWithEmployer = entity.DateSharedWithEmployer,
            EmployerFeedback = entity.EmployerFeedback,
            HasEverBeenEmployerInterviewing = entity.HasEverBeenEmployerInterviewing,
            Id = entity.Id,
            LegacyApplicationId = entity.LegacyApplicationId,
            Owner = entity.Owner,
            ReviewedDate = entity.ReviewedDate,
            Status = entity.Status,
            StatusUpdatedDate = entity.StatusUpdatedDate,
            SubmittedDate = entity.SubmittedDate,
            Ukprn = entity.Ukprn,
            VacancyReference = entity.VacancyReference,
            VacancyTitle = entity.VacancyTitle,
            WithdrawnDate = entity.WithdrawnDate,
        };
    }

    public static PutApplicationReviewResponse ToPutResponse(this ApplicationReviewEntity entity)
    {
        return new PutApplicationReviewResponse {
            AccountId = entity.AccountId,
            AccountLegalEntityId = entity.AccountLegalEntityId,
            AdditionalQuestion1 = entity.AdditionalQuestion1,
            AdditionalQuestion2 = entity.AdditionalQuestion2,
            ApplicationId = entity.ApplicationId,
            CandidateFeedback = entity.CandidateFeedback,
            CandidateId = entity.CandidateId,
            CreatedDate = entity.CreatedDate,
            DateSharedWithEmployer = entity.DateSharedWithEmployer,
            EmployerFeedback = entity.EmployerFeedback,
            HasEverBeenEmployerInterviewing = entity.HasEverBeenEmployerInterviewing,
            Id = entity.Id,
            LegacyApplicationId = entity.LegacyApplicationId,
            Owner = entity.Owner,
            ReviewedDate = entity.ReviewedDate,
            Status = entity.Status,
            StatusUpdatedDate = entity.StatusUpdatedDate,
            SubmittedDate = entity.SubmittedDate,
            Ukprn = entity.Ukprn,
            VacancyReference = entity.VacancyReference,
            VacancyTitle = entity.VacancyTitle,
            WithdrawnDate = entity.WithdrawnDate,
        };
    }
    
    public static PatchApplicationReviewResponse ToPatchResponse(this ApplicationReviewEntity entity)
    {
        return new PatchApplicationReviewResponse {
            AccountId = entity.AccountId,
            AccountLegalEntityId = entity.AccountLegalEntityId,
            AdditionalQuestion1 = entity.AdditionalQuestion1,
            AdditionalQuestion2 = entity.AdditionalQuestion2,
            ApplicationId = entity.ApplicationId,
            CandidateFeedback = entity.CandidateFeedback,
            CandidateId = entity.CandidateId,
            CreatedDate = entity.CreatedDate,
            DateSharedWithEmployer = entity.DateSharedWithEmployer,
            EmployerFeedback = entity.EmployerFeedback,
            HasEverBeenEmployerInterviewing = entity.HasEverBeenEmployerInterviewing,
            Id = entity.Id,
            LegacyApplicationId = entity.LegacyApplicationId,
            Owner = entity.Owner,
            ReviewedDate = entity.ReviewedDate,
            Status = entity.Status,
            StatusUpdatedDate = entity.StatusUpdatedDate,
            SubmittedDate = entity.SubmittedDate,
            Ukprn = entity.Ukprn,
            VacancyReference = entity.VacancyReference,
            VacancyTitle = entity.VacancyTitle,
            WithdrawnDate = entity.WithdrawnDate,
        };
    }

    public static ApplicationReviewEntity ToEntity(this PutApplicationReviewRequest request, Guid id)
    {
        return new ApplicationReviewEntity
        {
            Id = id,
            Ukprn = request.Ukprn,
            AccountId = request.AccountId,
            Owner = request.Owner,
            CandidateFeedback = request.CandidateFeedback,
            CandidateId = request.CandidateId,
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

    public static JsonPatchDocument<ApplicationReviewEntity> ToEntity(this JsonPatchDocument<ApplicationReview> source)
    {
        var result = new JsonPatchDocument<ApplicationReviewEntity>();
        var operations = source.Operations.Select(x => new Operation<ApplicationReviewEntity>
        {
            from = x.from,
            op = x.op,
            value = x.value,
            path = x?.path
        });
            
        result.Operations.AddRange(operations);
        return result;
    }
}