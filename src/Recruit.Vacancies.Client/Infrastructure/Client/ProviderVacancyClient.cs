using System;
using System.Linq;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Application.Commands;
using Esfa.Recruit.Vacancies.Client.Domain.Entities;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.Provider;
using Microsoft.Extensions.Logging;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.Client;

public partial class VacancyClient : IProviderVacancyClient
{
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

        await messaging.SendCommandAsync(command);

        await AssignVacancyNumber(id);
    }

    public async Task<long> GetVacancyCount(long ukprn, FilteringOptions? filteringOptions, string searchTerm)
    {
        return await vacancySummariesQuery.VacancyCount(ukprn, string.Empty, filteringOptions, searchTerm, OwnerType.Provider);
    }
        
    public async Task<ProviderDashboard> GetDashboardAsync(long ukprn,int page, FilteringOptions? status = null, string searchTerm = null)
    {
        var vacancySummariesTasks = vacancySummariesQuery.GetProviderOwnedVacancySummariesByUkprnAsync(ukprn, page, status, searchTerm);
        var transferredVacanciesTasks = vacancySummariesQuery.GetTransferredFromProviderAsync(ukprn);

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
            LastUpdated = timeProvider.Now
        };
    }

    private async Task UpdateWithTrainingProgrammeInfo(VacancySummary summary)
    {
        if (summary.ProgrammeId != null)
        {
            var programme = await apprenticeshipProgrammesProvider.GetApprenticeshipProgrammeAsync(summary.ProgrammeId);

            if (programme == null)
            {
                logger.LogWarning($"No training programme found for ProgrammeId: {summary.ProgrammeId}");
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