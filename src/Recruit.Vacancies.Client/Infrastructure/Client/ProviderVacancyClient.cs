using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Application.Commands;
using Esfa.Recruit.Vacancies.Client.Domain.Entities;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.EditVacancyInfo;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.Provider;
using Microsoft.Extensions.Logging;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.Client
{
    public partial class VacancyClient : IProviderVacancyClient
    {
        public async Task<Guid> CreateVacancyAsync(string employerAccountId,
            long ukprn, string title, VacancyUser user, string accountLegalEntityPublicHashedId, string legalEntityName)
        {
            var vacancyId = GenerateVacancyId();

            var command = new CreateProviderOwnedVacancyCommand(
                vacancyId,
                SourceOrigin.ProviderWeb,
                ukprn,
                employerAccountId,
                user,
                UserType.Provider,
                title,
                accountLegalEntityPublicHashedId,
                legalEntityName
            );

            await _messaging.SendCommandAsync(command);

            return vacancyId;
        }

        public async Task CreateProviderApiVacancy(Guid id, string title, string employerAccountId, VacancyUser user)
        {
            var command = new CreateProviderOwnedVacancyCommand(
                id,
                SourceOrigin.Api,
                user.Ukprn.Value,
                employerAccountId,
                user,
                UserType.Provider,
                title,
                null,
                null
            );

            await _messaging.SendCommandAsync(command);

            await AssignVacancyNumber(id);
        }

        public async Task<long> GetVacancyCount(long ukprn, FilteringOptions? filteringOptions, string searchTerm)
        {
            return await _vacancySummariesQuery.VacancyCount(ukprn, string.Empty, filteringOptions, searchTerm, OwnerType.Provider);
        }
        
        public async Task<ProviderDashboard> GetDashboardAsync(long ukprn,int page, FilteringOptions? status = null, string searchTerm = null)
        {
            var vacancySummariesTasks = _vacancySummariesQuery.GetProviderOwnedVacancySummariesByUkprnAsync(ukprn, page, status, searchTerm);
            var transferredVacanciesTasks = _vacancySummariesQuery.GetTransferredFromProviderAsync(ukprn);

            await Task.WhenAll(vacancySummariesTasks, transferredVacanciesTasks);

            var vacancySummaries = vacancySummariesTasks.Result
                .Where(c=> !c.IsTraineeship).ToList();
            var transferredVacancies = transferredVacanciesTasks.Result.Select(t =>
                new ProviderDashboardTransferredVacancy
                {
                    LegalEntityName = t.LegalEntityName,
                    TransferredDate = t.TransferredDate,
                    Reason = t.Reason
                });

            foreach (var summary in vacancySummaries)
            {
                await UpdateWithTrainingProgrammeInfo(summary);
            }

            return new ProviderDashboard
            {
                Id = QueryViewType.ProviderDashboard.GetIdValue(ukprn),
                Vacancies = vacancySummaries,
                TransferredVacancies = transferredVacancies,
                LastUpdated = _timeProvider.Now
            };
        }

        private async Task UpdateWithTrainingProgrammeInfo(VacancySummary summary)
        {
            if (summary.ProgrammeId != null)
            {
                var programme = await _apprenticeshipProgrammesProvider.GetApprenticeshipProgrammeAsync(summary.ProgrammeId);

                if (programme == null)
                {
                    _logger.LogWarning($"No training programme found for ProgrammeId: {summary.ProgrammeId}");
                }
                else
                {
                    summary.TrainingTitle = programme.Title;
                    summary.TrainingType = programme.ApprenticeshipType;
                    summary.TrainingLevel = programme.ApprenticeshipLevel;
                }
            }
        }

    }
}