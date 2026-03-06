namespace SFA.DAS.Recruit.Api.Core.Events;

public sealed record VacancyReviewCreatedEvent(Guid VacancyId, Guid VacancyReviewId);