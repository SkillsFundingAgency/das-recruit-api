using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models.Requests.ApplicationReview;
using SFA.DAS.Recruit.Api.Models.Responses.ApplicationReview;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

internal static class ApplicationReviewExtensions
{
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
            ReviewedDate = entity.ReviewedDate,
            Status = GetApplicationReviewStatusOrDefault(entity.Status),
            TemporaryReviewStatus = entity.TemporaryReviewStatus,
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
            ReviewedDate = entity.ReviewedDate,
            Status = GetApplicationReviewStatusOrDefault(entity.Status),
            TemporaryReviewStatus = entity.TemporaryReviewStatus,
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
            ReviewedDate = entity.ReviewedDate,
            Status = GetApplicationReviewStatusOrDefault(entity.Status),
            TemporaryReviewStatus = entity.TemporaryReviewStatus,
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
            CandidateFeedback = request.CandidateFeedback,
            CandidateId = request.CandidateId,
            DateSharedWithEmployer = request.DateSharedWithEmployer,
            HasEverBeenEmployerInterviewing = request.HasEverBeenEmployerInterviewing,
            ReviewedDate = request.ReviewedDate,
            Status = request.Status,
            TemporaryReviewStatus = request.TemporaryReviewStatus,
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

    public static List<GetApplicationReviewResponse> ToGetResponse(this List<ApplicationReviewEntity> entities)
    {
        return entities.Select(ToGetResponse).ToList();
    }
    
    private static ApplicationReviewStatus GetApplicationReviewStatusOrDefault(string applicationReviewStatus)
    {
        if(!Enum.TryParse<ApplicationReviewStatus>(applicationReviewStatus,true, out var status))
        {
            status = ApplicationReviewStatus.New;
        }

        return status;
    }
}