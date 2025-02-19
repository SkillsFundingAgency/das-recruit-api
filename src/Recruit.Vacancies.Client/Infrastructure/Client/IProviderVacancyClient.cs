using System;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Domain.Entities;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.Provider;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.Client
{
    public interface IProviderVacancyClient
    {
        Task<ProviderDashboard> GetDashboardAsync(long ukprn,int page, FilteringOptions? status = null, string searchTerm = null);
        Task CreateProviderApiVacancy(Guid id, string title, string employerAccountId, VacancyUser user);
        Task<long> GetVacancyCount(long ukprn, FilteringOptions? filteringOptions, string searchTerm);
    }
}