using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.Providers;

public interface IVacancyProvider
{
    Task<VacancyDashboardModel> GetCountByAccountId(long accountId, CancellationToken token = default);
    Task<VacancyDashboardModel> GetCountByUkprn(int ukprn, CancellationToken token = default);
}

public class VacancyProvider(IVacancyRepository vacancyRepository) : IVacancyProvider
{
    private const int ClosingSoonDays = 5;

    public async Task<VacancyDashboardModel> GetCountByAccountId(long accountId, CancellationToken token = default)
    {
        var vacancies = await vacancyRepository.GetAllByAccountId(accountId, token);
        return GetDashboardModel(vacancies);
    }

    public async Task<VacancyDashboardModel> GetCountByUkprn(int ukprn, CancellationToken token = default)
    {
        var vacancies = await vacancyRepository.GetAllByUkprn(ukprn, token);
        return GetDashboardModel(vacancies);
    }

    private static VacancyDashboardModel GetDashboardModel(List<VacancyEntity> vacancyEntities)
    {
        var counts = vacancyEntities
            .GroupBy(v => v.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        var closingSoonThreshold = DateTime.UtcNow.AddDays(ClosingSoonDays);

        var closingSoonVacancies = vacancyEntities
            .Where(v => v.Status == VacancyStatus.Live && v.ClosingDate <= closingSoonThreshold)
            .ToList();

        return new VacancyDashboardModel
        {
            ClosedVacanciesCount = counts.GetValueOrDefault(VacancyStatus.Closed),
            DraftVacanciesCount = counts.GetValueOrDefault(VacancyStatus.Draft),
            ReviewVacanciesCount = counts.GetValueOrDefault(VacancyStatus.Review),
            ReferredVacanciesCount = counts.GetValueOrDefault(VacancyStatus.Referred)
                                     + counts.GetValueOrDefault(VacancyStatus.Rejected),
            LiveVacanciesCount = counts.GetValueOrDefault(VacancyStatus.Live),
            SubmittedVacanciesCount = counts.GetValueOrDefault(VacancyStatus.Submitted),
            ClosingSoonVacanciesCount = closingSoonVacancies.Count,
            ClosingSoonWithNoApplications = closingSoonVacancies
                .Count(v => v.ApplicationMethod == ApplicationMethod.ThroughFindAnApprenticeship)
        };
    }
}
