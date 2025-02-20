using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Application.Providers;
using Esfa.Recruit.Vacancies.Client.Domain.Entities;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.Employer;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.Provider;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.EditVacancyInfo;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.QA;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.Vacancy;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.VacancyApplications;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.VacancyAnalytics;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.BlockedOrganisations;
using Recruit.Vacancies.Client.Domain.Entities;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore
{
    public class QueryStoreClient : IQueryStoreReader, IQueryStoreWriter
    {
        private readonly IQueryStore _queryStore;
        private readonly ITimeProvider _timeProvider;

        public QueryStoreClient(IQueryStore queryStore, ITimeProvider timeProvider)
        {
            _queryStore = queryStore;
            _timeProvider = timeProvider;
        }
        
        public Task UpdateEmployerVacancyDataAsync(string employerAccountId, IEnumerable<LegalEntity> legalEntities)
        {
            var employerVacancyDataItem = new EmployerEditVacancyInfo
            {
                Id = QueryViewType.EditVacancyInfo.GetIdValue(employerAccountId),
                LegalEntities = legalEntities,
                LastUpdated = _timeProvider.Now
            };

            return _queryStore.UpsertAsync(employerVacancyDataItem);
        }

        public Task UpdateProviderVacancyDataAsync(long ukprn, IEnumerable<EmployerInfo> employers, bool hasAgreement)
        {
            var providerVacancyDataItem = new ProviderEditVacancyInfo
            {
                Id = QueryViewType.EditVacancyInfo.GetIdValue(ukprn),
                Employers = employers,
                HasProviderAgreement = hasAgreement,
                LastUpdated = _timeProvider.Now
            };

            return _queryStore.UpsertAsync(providerVacancyDataItem);
        }

        public Task UpdateLiveVacancyAsync(LiveVacancy vacancy)
        {
            return _queryStore.UpsertAsync(vacancy);
        }

        public Task<long> DeleteAllLiveVacancies()
        {
            return _queryStore.DeleteAllAsync<LiveVacancy>(QueryViewType.LiveVacancy.TypeName);
        }

        public Task<long> DeleteAllClosedVacancies()
        {
            return _queryStore.DeleteAllAsync<ClosedVacancy>(QueryViewType.ClosedVacancy.TypeName);
        }

        public Task UpdateClosedVacancyAsync(ClosedVacancy closedVacancy)
        {
            return _queryStore.UpsertAsync(closedVacancy);
        }

        public Task<IEnumerable<LiveVacancy>> GetAllLiveVacancies(int vacanciesToSkip, int vacanciesToGet)
        {
            return _queryStore.GetAllLiveVacancies(vacanciesToSkip, vacanciesToGet);
        }

        public Task<IEnumerable<LiveVacancy>> GetAllLiveVacanciesOnClosingDate(int vacanciesToSkip, int vacanciesToGet, DateTime closingDate)
        {
            return _queryStore.GetAllLiveVacanciesOnClosingDate(vacanciesToSkip, vacanciesToGet, closingDate);
        }

        public Task<long> GetAllLiveVacanciesCount()
        {
            return _queryStore.GetAllLiveVacanciesCount();
        }

        public Task<long> GetTotalPositionsAvailableCount()
        {
            return _queryStore.GetTotalPositionsAvailableCount();
        }

        public Task<long> GetAllLiveVacanciesOnClosingDateCount(DateTime closingDate)
        {
            return _queryStore.GetAllLiveVacanciesOnClosingDateCount(closingDate);
        }
        public Task<LiveVacancy> GetLiveVacancy(long vacancyReference)
        {
            return _queryStore.GetLiveVacancy(vacancyReference);
        }
    }
}