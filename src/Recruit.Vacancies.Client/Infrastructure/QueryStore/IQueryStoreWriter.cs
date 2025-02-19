using System.Collections.Generic;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.EditVacancyInfo;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.Vacancy;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore;

public interface IQueryStoreWriter
{
    Task UpdateEmployerVacancyDataAsync(string employerAccountId, IEnumerable<LegalEntity> legalEntities);
    Task UpdateProviderVacancyDataAsync(long ukprn, IEnumerable<EmployerInfo> employers, bool hasAgreement);
    Task UpdateLiveVacancyAsync(LiveVacancy vacancy);
    Task<long> DeleteAllLiveVacancies();
    Task<long> DeleteAllClosedVacancies();
    Task UpdateClosedVacancyAsync(ClosedVacancy closedVacancy);
}