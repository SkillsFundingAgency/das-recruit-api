using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Models;

public record ApplicationReview
{
    public Guid Id { get; init; }
    public Guid? ApplicationId { get; init; }
    public string VacancyTitle { get; init; }
    public DateTime CreatedDate {get; init; }
    public DateTime? SubmittedDate {get; init; } 
    public DateTime? DateSharedWithEmployer {get; init; }
    public DateTime? ReviewedDate {get; init; } 
    public DateTime? StatusUpdatedDate {get; init; } 
    public DateTime? WithdrawnDate {get; init; } 
    public Guid CandidateId {get; init; }
    public Guid? LegacyApplicationId {get; init; } 
    public bool HasEverBeenEmployerInterviewing {get; init; }
    public int Ukprn {get; init; }
    public long AccountId {get; init; }
    public long AccountLegalEntityId {get; init; }
    public long VacancyReference {get; init; }
    public ApplicationReviewStatus Status {get; init; } 
    public string? TemporaryReviewStatus { get; init; }
    public string? AdditionalQuestion1 {get; init; }
    public string? AdditionalQuestion2 {get; init; }
    public string? CandidateFeedback {get; init; }
    public string? EmployerFeedback {get; init; }
}