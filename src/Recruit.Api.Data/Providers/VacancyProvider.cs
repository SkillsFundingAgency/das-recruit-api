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
    Task<PaginatedList<VacancySummaryEntity>> GetPagedVacancyByAccountId<TKey>(long accountId,
        ushort page,
        ushort pageSize,
        Expression<Func<VacancyEntity, TKey>> orderBy,
        SortOrder sortOrder,
        FilteringOptions filteringOptions,
        string searchTerm,
        CancellationToken cancellationToken);
    Task<PaginatedList<VacancySummaryEntity>> GetPagedVacancyByUkprn<TKey>(int ukprn,
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
    public async Task<VacancyDashboardModel> GetCountByAccountId(long accountId, CancellationToken token = default)
    {
        var vacancies = await vacancyRepository.GetEmployerDashboard(accountId, token);
        var vacanciesClosingSoon = await vacancyRepository.GetEmployerVacanciesClosingSoonWithApplications(accountId, token);
        
        var model = (VacancyDashboardModel)vacancies;
        model.ClosingSoonVacanciesCount = vacanciesClosingSoon.Sum(c => c.Item1);
        model.ClosingSoonWithNoApplications = vacanciesClosingSoon.Where(c=>!c.Item2).Sum(c => c.Item1);
        return model;
    }

    public async Task<VacancyDashboardModel> GetCountByUkprn(int ukprn, CancellationToken token = default)
    {
        var vacancies = await vacancyRepository.GetProviderDashboard(ukprn, token);
        var vacanciesClosingSoon = await vacancyRepository.GetProviderVacanciesClosingSoonWithApplications(ukprn, token);
        
        var model = (VacancyDashboardModel)vacancies;
        model.ClosingSoonVacanciesCount = vacanciesClosingSoon.Sum(c => c.Item1);
        model.ClosingSoonWithNoApplications = vacanciesClosingSoon.Where(c=>!c.Item2).Sum(c => c.Item1);
        return model;
    }

    public async Task<PaginatedList<VacancySummaryEntity>> GetPagedVacancyByAccountId<TKey>(long accountId, ushort page, ushort pageSize, Expression<Func<VacancyEntity, TKey>> orderBy,
        SortOrder sortOrder, FilteringOptions filteringOptions, string searchTerm, CancellationToken cancellationToken)
    {
        return await vacancyRepository.GetManyByAccountIdAsync(accountId, page, pageSize, orderBy, sortOrder,
            filteringOptions, searchTerm, cancellationToken);
    }

    public async Task<PaginatedList<VacancySummaryEntity>> GetPagedVacancyByUkprn<TKey>(int ukprn, ushort page, ushort pageSize, Expression<Func<VacancyEntity, TKey>> orderBy,
        SortOrder sortOrder, FilteringOptions filteringOptions, string searchTerm, CancellationToken cancellationToken)
    {
        return await vacancyRepository.GetManyByUkprnIdAsync(ukprn, page, pageSize, orderBy, sortOrder,
            filteringOptions, searchTerm, cancellationToken);
    }
}
