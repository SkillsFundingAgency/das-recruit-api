namespace SFA.DAS.Recruit.Api.Core.Events;

public sealed record VacancyLiveEvent(Guid VacancyId, long VacancyReference);