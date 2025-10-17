using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Models.Responses.Vacancy;

public record VacancySummaryResponse(PageInfo PageInfo, IEnumerable<VacancySummary> VacancySummaries);