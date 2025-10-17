using System.Linq.Expressions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Domain.Models;

namespace SFA.DAS.Recruit.Api.Data.Providers;

public interface IVacancyProvider
{
    Task<VacancyDashboardModel> GetCountByAccountId(long accountId, CancellationToken token = default);
    Task<VacancyDashboardModel> GetCountByUkprn(int ukprn, CancellationToken token = default);
    Task<PaginatedList<VacancyEntity>> GetPagedVacancyByAccountId<TKey>(long accountId,
        ushort page,
        ushort pageSize,
        Expression<Func<VacancyEntity, TKey>> orderBy,
        SortOrder sortOrder,
        FilteringOptions filteringOptions,
        string searchTerm,
        CancellationToken cancellationToken);
    Task<PaginatedList<VacancyEntity>> GetPagedVacancyByUkprn<TKey>(int ukprn,
        ushort page,
        ushort pageSize,
        Expression<Func<VacancyEntity, TKey>> orderBy,
        SortOrder sortOrder,
        FilteringOptions filteringOptions,
        string searchTerm,
        CancellationToken cancellationToken);
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

    public async Task<PaginatedList<VacancyEntity>> GetPagedVacancyByAccountId<TKey>(long accountId, ushort page, ushort pageSize, Expression<Func<VacancyEntity, TKey>> orderBy,
        SortOrder sortOrder, FilteringOptions filteringOptions, string searchTerm, CancellationToken cancellationToken)
    {
        return await vacancyRepository.GetManyByAccountIdAsync(accountId, page, pageSize, orderBy, sortOrder,
            filteringOptions, searchTerm, cancellationToken);
    }

    public async Task<PaginatedList<VacancyEntity>> GetPagedVacancyByUkprn<TKey>(int ukprn, ushort page, ushort pageSize, Expression<Func<VacancyEntity, TKey>> orderBy,
        SortOrder sortOrder, FilteringOptions filteringOptions, string searchTerm, CancellationToken cancellationToken)
    {
        return await vacancyRepository.GetManyByUkprnIdAsync(ukprn, page, pageSize, orderBy, sortOrder,
            filteringOptions, searchTerm, cancellationToken);
    }

    private static VacancyDashboardModel GetDashboardModel(List<VacancyEntity> vacancyEntities)
    {
        if (vacancyEntities.Count == 0)
            return new VacancyDashboardModel();

        var now = DateTime.UtcNow;
        var threshold = now.AddDays(ClosingSoonDays);

        int closed = 0, draft = 0, review = 0, referred = 0, rejected = 0, live = 0, submitted = 0;
        int closingSoon = 0, closingSoonWithNoApps = 0;

        foreach (var v in vacancyEntities)
        {
            switch (v.Status)
            {
                case VacancyStatus.Closed: closed++; break;
                case VacancyStatus.Draft: draft++; break;
                case VacancyStatus.Review: review++; break;
                case VacancyStatus.Referred: referred++; break;
                case VacancyStatus.Rejected: rejected++; break;
                case VacancyStatus.Live:
                    live++;
                    if (v.ClosingDate <= threshold)
                    {
                        closingSoon++;
                        if (v.ApplicationMethod == ApplicationMethod.ThroughFindAnApprenticeship)
                            closingSoonWithNoApps++;
                    }
                    break;
                case VacancyStatus.Submitted: submitted++; break;
            }
        }

        return new VacancyDashboardModel {
            ClosedVacanciesCount = closed,
            DraftVacanciesCount = draft,
            ReviewVacanciesCount = review,
            ReferredVacanciesCount = referred + rejected,
            LiveVacanciesCount = live,
            SubmittedVacanciesCount = submitted,
            ClosingSoonVacanciesCount = closingSoon,
            ClosingSoonWithNoApplications = closingSoonWithNoApps
        };
    }
}
