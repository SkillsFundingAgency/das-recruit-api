using System;

namespace SFA.DAS.Recruit.Api.Models.Responses;

public sealed record ApplicationReviewResponse(Guid Id,
    int Ukprn,
    long AccountId,
    long AccountLegalEntityId,
    short Owner,
    string? CandidateFeedback,
    string? EmployerFeedback,
    Guid CandidateId,
    DateTime CreatedDate,
    DateTime? DateSharedWithEmployer,
    bool HasEverBeenEmployerInterviewing,
    DateTime? WithdrawnDate,
    DateTime? ReviewedDate,
    DateTime SubmittedDate,
    string Status,
    DateTime? StatusUpdatedDate,
    long VacancyReference,
    Guid? LegacyApplicationId,
    Guid? ApplicationId,
    string? AdditionalQuestion1,
    string? AdditionalQuestion2,
    string VacancyTitle
);