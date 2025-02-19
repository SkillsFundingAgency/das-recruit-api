using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Application;
using Esfa.Recruit.Vacancies.Client.Application.Commands;
using Esfa.Recruit.Vacancies.Client.Application.Providers;
using Esfa.Recruit.Vacancies.Client.Application.Validation;
using Esfa.Recruit.Vacancies.Client.Domain.Entities;
using Esfa.Recruit.Vacancies.Client.Domain.Messaging;
using Esfa.Recruit.Vacancies.Client.Domain.Repositories;
using Esfa.Recruit.Vacancies.Client.Infrastructure.OuterApi.Responses;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.EditVacancyInfo;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.Employer;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.VacancyAnalytics;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.VacancyApplications;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Services.EmployerAccount;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Services.VacancySummariesProvider;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.Client
{
    public partial class VacancyClient : IRecruitVacancyClient, IEmployerVacancyClient
    {
        private readonly ILogger<VacancyClient> _logger;
        private readonly IMessaging _messaging;
        private readonly IQueryStoreReader _reader;
        private readonly IVacancyRepository _repository;
        private readonly IEntityValidator<Vacancy, VacancyRuleSet> _validator;
        private readonly IApprenticeshipProgrammeProvider _apprenticeshipProgrammesProvider;
        private readonly IEmployerAccountProvider _employerAccountProvider;
        private readonly ICandidateSkillsProvider _candidateSkillsProvider;
        private readonly IEmployerProfileRepository _employerProfileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IQualificationsProvider _qualificationsProvider;
        private readonly IVacancySummariesProvider _vacancySummariesQuery;
        private readonly ITimeProvider _timeProvider;

        public VacancyClient(
            ILogger<VacancyClient> logger,
            IVacancyRepository repository,
            IQueryStoreReader reader,
            IMessaging messaging,
            IEntityValidator<Vacancy, VacancyRuleSet> validator,
            IApprenticeshipProgrammeProvider apprenticeshipProgrammesProvider,
            IEmployerAccountProvider employerAccountProvider,
            ICandidateSkillsProvider candidateSkillsProvider,
            IEmployerProfileRepository employerProfileRepository,
            IUserRepository userRepository,
            IQualificationsProvider qualificationsProvider, 
            IVacancySummariesProvider vacancySummariesQuery, 
            ITimeProvider timeProvider)
        {
            _logger = logger;
            _repository = repository;
            _reader = reader;
            _messaging = messaging;
            _validator = validator;
            _apprenticeshipProgrammesProvider = apprenticeshipProgrammesProvider;
            _employerAccountProvider = employerAccountProvider;
            _candidateSkillsProvider = candidateSkillsProvider;
            _employerProfileRepository = employerProfileRepository;
            _userRepository = userRepository;
            _qualificationsProvider = qualificationsProvider;
            _vacancySummariesQuery = vacancySummariesQuery;
            _timeProvider = timeProvider;
        }

        public Task<Vacancy> GetVacancyAsync(Guid vacancyId)
        {
            return _repository.GetVacancyAsync(vacancyId);
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
            
            await _messaging.SendCommandAsync(command);
            
            await AssignVacancyNumber(id);
        }

        private Guid GenerateVacancyId()
        {
            return Guid.NewGuid();
        }

        public async Task<EmployerDashboard> GetDashboardAsync(string employerAccountId, int page, FilteringOptions? status = null, string searchTerm = null)
        {
            var vacancySummaries =
                await _vacancySummariesQuery.GetEmployerOwnedVacancySummariesByEmployerAccountId(employerAccountId,
                     page, status, searchTerm);
            return new EmployerDashboard
            {
                Id = QueryViewType.EmployerDashboard.GetIdValue(employerAccountId),
                Vacancies = vacancySummaries,
                LastUpdated = _timeProvider.Now
            };
        }

        public Task<EmployerEditVacancyInfo> GetEditVacancyInfoAsync(string employerAccountId)
        {
            return _reader.GetEmployerVacancyDataAsync(employerAccountId);
        }

        public EntityValidationResult Validate(Vacancy vacancy, VacancyRuleSet rules)
        {
            return _validator.Validate(vacancy, rules);
        }

        public Task<GetUserAccountsResponse> GetEmployerIdentifiersAsync(string userId, string email)
        {
            return _employerAccountProvider.GetEmployerIdentifiersAsync(userId, email);
        }

        public Task<List<string>> GetCandidateSkillsAsync()
        {
            return _candidateSkillsProvider.GetCandidateSkillsAsync();
        }

        public Task<IList<string>> GetCandidateQualificationsAsync()
        {
            return _qualificationsProvider.GetQualificationsAsync();
        }

        public Task<EmployerProfile> GetEmployerProfileAsync(string employerAccountId, string accountLegalEntityPublicHashedId)
        {
            return _employerProfileRepository.GetAsync(employerAccountId, accountLegalEntityPublicHashedId);
        }

        public Task UpdateEmployerProfileAsync(EmployerProfile employerProfile, VacancyUser user)
        {
            var command = new UpdateEmployerProfileCommand
            {
                Profile = employerProfile,
                User = user
            };

            return _messaging.SendCommandAsync(command);
        }

        // Jobs
        public Task AssignVacancyNumber(Guid vacancyId)
        {
            var command = new AssignVacancyNumberCommand
            {
                VacancyId = vacancyId
            };

            return _messaging.SendCommandAsync(command);
        }

        public Task<User> GetUsersDetailsAsync(string userId)
        {
            return _userRepository.GetAsync(userId);
        }

        
        public async Task<long> GetVacancyCount(string employerAccountId, FilteringOptions? filteringOptions, string searchTerm)
        {
            var ownerType = (filteringOptions == FilteringOptions.NewSharedApplications || filteringOptions == FilteringOptions.AllSharedApplications) ? OwnerType.Provider : OwnerType.Employer;
            return await _vacancySummariesQuery.VacancyCount(null, employerAccountId, filteringOptions, searchTerm, ownerType);
        }
    }
}