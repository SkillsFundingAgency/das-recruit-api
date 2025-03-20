using System;
using Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Models.Responses;

public sealed record ApplicationReviewResponse(
    Guid Id,
    int Ukprn,
    long AccountId,
    short Owner,
    string? CandidateFeedback,
    Guid CandidateId,
    DateTime CreatedDate,
    DateTime? DateSharedWithEmployer,
    bool HasEverBeenEmployerInterviewing,
    bool IsWithdrawn,
    DateTime? ReviewedDate,
    DateTime SubmittedDate,
    ApplicationStatus Status,
    Guid StatusUpdatedByUserId,
    short? StatusUpdatedBy,
    long VacancyReference,
    Guid LegacyApplicationId
);