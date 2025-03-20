using System;
using Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Models.Requests;

public record ApplicationReviewRequest
{
    public required Guid Id { get; init; }
    public required int Ukprn { get; init; }
    public required long AccountId { get; init; }
    public required short Owner { get; init; }
    public string? CandidateFeedback { get; init; }
    public required Guid CandidateId { get; init; }
    public required DateTime CreatedDate { get; init; }
    public DateTime? DateSharedWithEmployer { get; init; }
    public bool HasEverBeenEmployerInterviewing { get; init; }
    public bool IsWithdrawn { get; init; }
    public DateTime? ReviewedDate { get; init; }
    public required DateTime SubmittedDate { get; init; }
    public ApplicationStatus Status { get; init; }
    public required Guid StatusUpdatedByUserId { get; init; }
    public short? StatusUpdatedBy { get; init; }
    public required long VacancyReference { get; init; }
    public required Guid LegacyApplicationId { get; init; }
}