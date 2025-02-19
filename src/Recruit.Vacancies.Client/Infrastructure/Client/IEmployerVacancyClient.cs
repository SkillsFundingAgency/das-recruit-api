using System;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Domain.Entities;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.Employer;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.EditVacancyInfo;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.Client
{
    public interface IEmployerVacancyClient
    {
        Task<EmployerDashboard> GetDashboardAsync(string employerAccountId,int page, FilteringOptions? status = null, string searchTerm = null);
        Task<EmployerEditVacancyInfo> GetEditVacancyInfoAsync(string employerAccountId);
        Task CreateEmployerApiVacancy(Guid id, string title, string employerAccountId, VacancyUser user, TrainingProvider provider, string programmeId);
        Task<long> GetVacancyCount(string employerAccountId, FilteringOptions? filteringOptions, string searchTerm);
        
    }
}