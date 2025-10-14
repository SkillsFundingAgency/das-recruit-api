using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Models.Requests.Vacancy;

public record PostClosedVacanciesRequest
{
    public required List<long> VacancyReferences { get; init; } = [];
}