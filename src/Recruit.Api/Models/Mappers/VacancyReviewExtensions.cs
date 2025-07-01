using System.Text.Json;
using SFA.DAS.Recruit.Api.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models.Requests.VacancyReview;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class VacancyReviewExtensions
{
    private static VacancyReview ToResponseDto(this VacancyReviewEntity entity)
    {
        return new VacancyReview {
            AutomatedQaOutcome = entity.AutomatedQaOutcome,
            AutomatedQaOutcomeIndicators = entity.AutomatedQaOutcomeIndicators,
            ClosedDate = entity.ClosedDate,
            CreatedDate = entity.CreatedDate,
            DismissedAutomatedQaOutcomeIndicators = JsonSerializer.Deserialize<List<string>>(entity.DismissedAutomatedQaOutcomeIndicators, JsonConfig.Options) ?? [],
            Id = entity.Id,
            ManualOutcome = entity.ManualOutcome,
            ManualQaComment = entity.ManualQaComment,
            ManualQaFieldIndicators = JsonSerializer.Deserialize<List<string>>(entity.ManualQaFieldIndicators, JsonConfig.Options) ?? [],
            ReviewedByUserEmail = entity.ReviewedByUserEmail,
            ReviewedDate = entity.ReviewedDate,
            SlaDeadLine = entity.SlaDeadLine,
            Status = entity.Status,
            SubmissionCount = entity.SubmissionCount,
            SubmittedByUserEmail = entity.SubmittedByUserEmail,
            UpdatedFieldIdentifiers = JsonSerializer.Deserialize<List<string>>(entity.UpdatedFieldIdentifiers, JsonConfig.Options) ?? [],
            VacancyReference = entity.VacancyReference,
            VacancySnapshot = entity.VacancySnapshot,
            VacancyTitle = entity.VacancyTitle,
            OwnerType = entity.OwnerType,
            AccountId = entity.AccountId,
            AccountLegalEntityId = entity.AccountLegalEntityId,
            Ukprn = entity.Ukprn
        };
    }
    
    public static VacancyReview ToGetResponse(this VacancyReviewEntity entity)
    {
        return ToResponseDto(entity);
    }
    
    public static List<VacancyReview> ToGetResponse(this List<VacancyReviewEntity> entities)
    {
        return entities.Select(ToResponseDto).ToList();
    }
    
    public static VacancyReview ToPutResponse(this VacancyReviewEntity entity)
    {
        return entity.ToResponseDto();
    }
    
    public static VacancyReview ToPatchResponse(this VacancyReviewEntity entity)
    {
        return entity.ToResponseDto();
    }
    
    public static VacancyReviewEntity ToDomain(this PutVacancyReviewRequest request, Guid id)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new VacancyReviewEntity {
            Id = id,
            VacancyReference = request.VacancyReference!.Value.Value,
            VacancyTitle = request.VacancyTitle,
            CreatedDate = request.CreatedDate!.Value,
            SlaDeadLine = request.SlaDeadLine!.Value,
            ReviewedDate = request.ReviewedDate,
            Status = request.Status,
            SubmissionCount = request.SubmissionCount!.Value,
            ReviewedByUserEmail = request.ReviewedByUserEmail,
            SubmittedByUserEmail = request.SubmittedByUserEmail,
            ClosedDate = request.ClosedDate,
            ManualOutcome = request.ManualOutcome,
            ManualQaComment = request.ManualQaComment,
            ManualQaFieldIndicators = JsonSerializer.Serialize(request.ManualQaFieldIndicators, JsonConfig.Options),
            AutomatedQaOutcome = request.AutomatedQaOutcome,
            AutomatedQaOutcomeIndicators = request.AutomatedQaOutcomeIndicators,
            DismissedAutomatedQaOutcomeIndicators = JsonSerializer.Serialize(request.DismissedAutomatedQaOutcomeIndicators, JsonConfig.Options),
            UpdatedFieldIdentifiers = JsonSerializer.Serialize(request.UpdatedFieldIdentifiers, JsonConfig.Options),
            VacancySnapshot = request.VacancySnapshot,
            Ukprn = request.Ukprn,
            AccountId = request.AccountId,
            OwnerType = request.OwnerType,
            AccountLegalEntityId = request.AccountLegalEntityId
        };
    }
}