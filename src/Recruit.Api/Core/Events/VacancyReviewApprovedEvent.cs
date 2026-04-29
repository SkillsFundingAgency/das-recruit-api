namespace SFA.DAS.Recruit.Api.Core.Events;

public sealed record VacancyReviewApprovedEvent(Guid VacancyReviewId, Guid VacancyId);