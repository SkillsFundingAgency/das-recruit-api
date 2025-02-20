using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Application.Commands;
using Esfa.Recruit.Vacancies.Client.Application.Providers;
using Esfa.Recruit.Vacancies.Client.Application.Validation;
using Esfa.Recruit.Vacancies.Client.Domain.Entities;
using Esfa.Recruit.Vacancies.Client.Domain.Messaging;
using Esfa.Recruit.Vacancies.Client.Domain.Repositories;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.Employer;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Services.VacancySummariesProvider;
using Microsoft.Extensions.Logging;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.Client;

public partial class VacancyClient(
    ILogger<VacancyClient> logger,
    IVacancyRepository repository,
    IMessaging messaging,
    IEntityValidator<Vacancy, VacancyRuleSet> validator,
    IApprenticeshipProgrammeProvider apprenticeshipProgrammesProvider,
    ICandidateSkillsProvider candidateSkillsProvider,
    IEmployerProfileRepository employerProfileRepository,
    IQualificationsProvider qualificationsProvider,
    IVacancySummariesProvider vacancySummariesQuery,
    ITimeProvider timeProvider)
    : IRecruitVacancyClient, IEmployerVacancyClient
{
    public Task<Vacancy> GetVacancyAsync(Guid vacancyId)
    {
        return repository.GetVacancyAsync(vacancyId);
    }

    public async Task CreateEmployerApiVacancy(Guid id, string title, string employerAccountId, VacancyUser user,
        TrainingProvider provider, string programmeId)
    {
        var command = new CreateEmployerOwnedVacancyCommand
        {
            VacancyId = id,
            User = user,
            UserType = UserType.Employer,
            Title = title,
            EmployerAccountId = employerAccountId,
            Origin = SourceOrigin.Api,
            ProgrammeId = programmeId,
            TrainingProvider = provider
        };
            
        await messaging.SendCommandAsync(command);
            
        await AssignVacancyNumber(id);
    }

    public async Task<EmployerDashboard> GetDashboardAsync(string employerAccountId, int page, FilteringOptions? status = null, string searchTerm = null)
    {
        var vacancySummaries =
            await vacancySummariesQuery.GetEmployerOwnedVacancySummariesByEmployerAccountId(employerAccountId,
                page, status, searchTerm);
        return new EmployerDashboard
        {
            Id = QueryViewType.EmployerDashboard.GetIdValue(employerAccountId),
            Vacancies = vacancySummaries,
            LastUpdated = timeProvider.Now
        };
    }

    public EntityValidationResult Validate(Vacancy vacancy, VacancyRuleSet rules)
    {
        return validator.Validate(vacancy, rules);
    }

    public Task<List<string>> GetCandidateSkillsAsync()
    {
        return candidateSkillsProvider.GetCandidateSkillsAsync();
    }

    public Task<IList<string>> GetCandidateQualificationsAsync()
    {
        return qualificationsProvider.GetQualificationsAsync();
    }

    public Task<EmployerProfile> GetEmployerProfileAsync(string employerAccountId, string accountLegalEntityPublicHashedId)
    {
        return employerProfileRepository.GetAsync(employerAccountId, accountLegalEntityPublicHashedId);
    }

    public Task UpdateEmployerProfileAsync(EmployerProfile employerProfile, VacancyUser user)
    {
        var command = new UpdateEmployerProfileCommand
        {
            Profile = employerProfile,
            User = user
        };

        return messaging.SendCommandAsync(command);
    }

    // Jobs
    public Task AssignVacancyNumber(Guid vacancyId)
    {
        var command = new AssignVacancyNumberCommand
        {
            VacancyId = vacancyId
        };

        return messaging.SendCommandAsync(command);
    }
        
    public async Task<long> GetVacancyCount(string employerAccountId, FilteringOptions? filteringOptions, string searchTerm)
    {
        var ownerType = (filteringOptions == FilteringOptions.NewSharedApplications || filteringOptions == FilteringOptions.AllSharedApplications) ? OwnerType.Provider : OwnerType.Employer;
        return await vacancySummariesQuery.VacancyCount(null, employerAccountId, filteringOptions, searchTerm, ownerType);
    }
}