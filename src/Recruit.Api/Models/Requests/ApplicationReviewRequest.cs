using System;

namespace SFA.DAS.Recruit.Api.Models.Requests;

public record ApplicationReviewRequest
{
    public required bool HasEverBeenEmployerInterviewing { get; init; }
    public required DateTime CreatedDate { get; set; }
    public required DateTime SubmittedDate { get; init; }
    public DateTime WithdrawnDate { get; set; }
    public DateTime? DateSharedWithEmployer { get; init; }
    public DateTime? ReviewedDate { get; init; }
    public DateTime? StatusUpdatedDate { get; init; }
    public required Guid CandidateId { get; init; }
    public required Guid Id { get; set; }
    public Guid? ApplicationId { get; init; }
    public Guid? LegacyApplicationId { get; init; }
    public required int Ukprn { get; set; }
    public required long AccountId { get; set; }
    public required long AccountLegalEntityId { get; set; }
    public required long VacancyReference { get; init; }
    public required string VacancyTitle { get; init; }
    public required short Owner { get; init; }
    public string? AdditionalQuestion1 { get; init; }
    public string? AdditionalQuestion2 { get; init; }
    public string? CandidateFeedback { get; init; }
    public string? EmployerFeedback { get; init; }
    public required string Status { get; init; }
}